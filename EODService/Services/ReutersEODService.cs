using EODService.DTOs.EOD;
using EODService.DTOs.Provider;
using EODService.DTOs.ReuterSettings;
using EODService.DTOs.SymbolSettings;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace EODService.Services
{
    public class ReutersEODService : IEODService
    {
        private readonly ProviderDTO _providerSettings;
        private readonly SymbolSettings _symbolSettings;
        private readonly ILogger<ReutersEODService> _logger;
        private readonly ReutersParametersDTO _parameters;

        // HARDCODED — not provided by the DB. Matches the working
        // standalone sample exactly. Ask your LSEG/RTDS admin if this
        // ever needs to be a real machine-specific value.
        private const string Position = "127.0.0.1";

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };

        public ReutersEODService(
            ProviderDTO providerSettings,
            SymbolSettings symbolSettings,
            ILogger<ReutersEODService> logger)
        {
            _providerSettings = providerSettings;
            _symbolSettings = symbolSettings;
            _logger = logger;

            // Parameters are stored as JSON in PROVIDER.PARAMETERS (DB)
            _parameters =
                JsonSerializer.Deserialize<ReutersParametersDTO>(
                    providerSettings.Parameters ?? "{}",
                    _jsonOptions)
                ?? new ReutersParametersDTO();

            _logger.LogInformation(
                "Reuters parameters loaded — DacsUser: '{DacsUser}', ApplicationId: '{AppId}', ServiceName: '{Service}'",
                _parameters.DacsUser,
                _parameters.ApplicationId,
                _parameters.ServiceName);
        }

        public async Task<List<EodData>> GetEodDataAsync()
        {
            var results = new List<EodData>();

            // BaseUrl + EndPoint come from the DB (PROVIDER table)
            var baseUrl = (_providerSettings.BaseUrl ?? string.Empty).TrimEnd('/');
            var endPoint = (_providerSettings.EndPoint ?? string.Empty).TrimStart('/');
            var wsUri = $"{baseUrl}/{endPoint}";

            using var socket = new ClientWebSocket();

            // LSEG WebSocket protocol
            socket.Options.AddSubProtocol("tr_json2");

            try
            {
                // =====================================================
                // 1. CONNECT
                // =====================================================

                _logger.LogInformation("Connecting to {Uri}...", wsUri);

                using (var connectCts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                {
                    await socket.ConnectAsync(new Uri(wsUri), connectCts.Token);
                }

                _logger.LogInformation("WebSocket connected.");

                // =====================================================
                // 2. LOGIN
                // =====================================================

                var resolvedPosition = !string.IsNullOrWhiteSpace(_parameters.Position)
                    ? _parameters.Position
                    : GetLocalHostIp();

                var loginRequest = new
                {
                    ID = 1,
                    Domain = "Login",

                    Key = new
                    {
                        Name = _parameters.DacsUser,       // from DB

                        Elements = new
                        {
                            ApplicationId = _parameters.ApplicationId,  // from DB
                            Position = resolvedPosition
                        }
                    }
                };

                await SendJsonAsync(socket, loginRequest);

                _logger.LogDebug("LOGIN REQUEST: {Json}", JsonSerializer.Serialize(loginRequest));

                // =====================================================
                // 3. WAIT FOR LOGIN RESPONSE
                // =====================================================

                bool loginSuccessful = false;

                while (socket.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    var json = await ReceiveMessageAsync(socket);

                    _logger.LogDebug("RECEIVED: {Json}", json);

                    using var document = JsonDocument.Parse(json);
                    var messages = ParseMessageElements(document);

                    // Auto-reply Pong to any Ping messages
                    await ProcessPingMessagesAsync(socket, messages, _logger);

                    bool stopLoginLoop = false;

                    foreach (var message in messages)
                    {
                        var domain = GetString(message, "Domain");
                        var type = GetString(message, "Type");

                        if (domain == "Login" && string.Equals(type, "Refresh", StringComparison.OrdinalIgnoreCase))
                        {
                            if (message.TryGetProperty("State", out var state))
                            {
                                var data = GetString(state, "Data");

                                if (string.Equals(data, "Ok", StringComparison.OrdinalIgnoreCase))
                                {
                                    loginSuccessful = true;
                                }
                                else
                                {
                                    _logger.LogWarning("Reuters DACS login state: {State}", data);
                                }
                            }

                            stopLoginLoop = true;
                            break;
                        }
                        else if (domain == "Login" && string.Equals(type, "Status", StringComparison.OrdinalIgnoreCase))
                        {
                            if (message.TryGetProperty("State", out var state))
                            {
                                var data = GetString(state, "Data");
                                var text = GetString(state, "Text");

                                _logger.LogWarning("Login status state: Data={Data}, Text={Text}", data, text);

                                if (!string.Equals(data, "Ok", StringComparison.OrdinalIgnoreCase))
                                {
                                    stopLoginLoop = true;
                                    break;
                                }
                            }
                        }
                        else if (string.Equals(type, "Error", StringComparison.OrdinalIgnoreCase))
                        {
                            var text = GetString(message, "Text");
                            _logger.LogError("Reuters LOGIN ERROR: {Text}", text);
                            stopLoginLoop = true;
                            break;
                        }
                    }

                    if (loginSuccessful || stopLoginLoop)
                        break;
                }

                // =====================================================
                // 4. CHECK LOGIN
                // =====================================================

                if (!loginSuccessful)
                {
                    _logger.LogError("Reuters DACS authentication failed.");
                    return results;
                }

                _logger.LogInformation("Reuters DACS login successful.");

                // =====================================================
                // 5. REQUEST SNAPSHOT FOR EACH SYMBOL
                // =====================================================

                for (int i = 0; i < _symbolSettings.Symbols.Count; i++)
                {
                    var symbol = _symbolSettings.Symbols[i];   // from DB
                    var id = _symbolSettings.Ids[i];           // from DB
                    var name = _symbolSettings.Names[i];       // from DB

                    int requestId = i + 2;

                    try
                    {
                        var marketPriceRequest = new
                        {
                            ID = requestId,

                            Key = new
                            {
                                Service = _parameters.ServiceName,  // from DB
                                Name = symbol
                            },

                            // We only want one snapshot.
                            Streaming = false,

                            View = new[]
                            {
                                "TRADE_DATE",
                                "OPEN_PRC",
                                "HIGH_1",
                                "LOW_1",
                                "OFF_CLOSE",
                                "HST_CLOSE",
                                "TRDPRC_1",
                                "ADJUST_CLS",
                                "ACVOL_1"
                            }
                        };

                        await SendJsonAsync(socket, marketPriceRequest);

                        _logger.LogDebug(
                            "MARKET PRICE REQUEST (ID={RequestId}): {Json}",
                            requestId,
                            JsonSerializer.Serialize(marketPriceRequest));

                        // =================================================
                        // 6. RECEIVE EOD RESPONSE
                        // =================================================

                        bool snapshotReceived = false;

                        while (!snapshotReceived && socket.State == System.Net.WebSockets.WebSocketState.Open)
                        {
                            var json = await ReceiveMessageAsync(socket);

                            _logger.LogDebug("RECEIVED: {Json}", json);

                            using var document = JsonDocument.Parse(json);
                            var messages = ParseMessageElements(document);

                            // Auto-reply Pong to any Ping messages
                            await ProcessPingMessagesAsync(socket, messages, _logger);

                            foreach (var message in messages)
                            {
                                // Match message ID to requestId
                                if (message.TryGetProperty("ID", out var idProp) && idProp.TryGetInt32(out int msgId))
                                {
                                    if (msgId != requestId)
                                    {
                                        // Belongs to a different request (e.g. Login ID=1), ignore and continue waiting
                                        continue;
                                    }
                                }
                                else
                                {
                                    // No numeric ID (e.g. system/ping message), skip matching for snapshot
                                    continue;
                                }

                                var type = GetString(message, "Type");

                                if (string.Equals(type, "Refresh", StringComparison.OrdinalIgnoreCase))
                                {
                                    _logger.LogDebug(
                                        "EOD SNAPSHOT RECEIVED for {Symbol} (ID={RequestId}): {Json}",
                                        symbol,
                                        requestId,
                                        message.GetRawText());

                                    WebSocketResponse? response = null;

                                    try
                                    {
                                        response = message.Deserialize<WebSocketResponse>(_jsonOptions);
                                    }
                                    catch (JsonException ex)
                                    {
                                        _logger.LogError(
                                            ex,
                                            "Unable to deserialize Reuters response for {Symbol}.",
                                            symbol);

                                        snapshotReceived = true;
                                        break;
                                    }

                                    if (response == null)
                                    {
                                        _logger.LogWarning(
                                            "Reuters returned an empty response for {Symbol}.",
                                            symbol);

                                        snapshotReceived = true;
                                        break;
                                    }

                                    var eodData = ReuterMapper.Map(response, id, name, msg => _logger.LogWarning("{Warning}", msg));

                                    if (eodData == null)
                                    {
                                        _logger.LogWarning(
                                            "ReuterMapper returned null for {Symbol}. Check received fields.",
                                            symbol);
                                    }
                                    else
                                    {
                                        results.Add(eodData);

                                        var openStr  = eodData.Open.HasValue  ? eodData.Open.Value.ToString("F4").PadLeft(9)  : "        -";
                                        var highStr  = eodData.High.HasValue  ? eodData.High.Value.ToString("F4").PadLeft(9)  : "        -";
                                        var lowStr   = eodData.Low.HasValue   ? eodData.Low.Value.ToString("F4").PadLeft(9)   : "        -";
                                        var closeStr = eodData.Close.HasValue ? eodData.Close.Value.ToString("F4").PadLeft(9) : "        -";
                                        var volStr   = eodData.Volume.HasValue ? eodData.Volume.Value.ToString("N0").PadLeft(12) : "           -";

                                        _logger.LogInformation(
                                            "✔ {Symbol,-12} │ Date: {Date:yyyy-MM-dd} │ Open:{Open} │ High:{High} │ Low:{Low} │ Close:{Close} │ Vol:{Vol}",
                                            symbol,
                                            eodData.Date,
                                            openStr,
                                            highStr,
                                            lowStr,
                                            closeStr,
                                            volStr);
                                    }

                                    snapshotReceived = true;
                                    break;
                                }
                                else if (string.Equals(type, "Status", StringComparison.OrdinalIgnoreCase))
                                {
                                    _logger.LogWarning("Reuters STATUS message received for {Symbol} (ID={RequestId}).", symbol, requestId);

                                    if (message.TryGetProperty("State", out var state))
                                    {
                                        var stream = GetString(state, "Stream");
                                        var data = GetString(state, "Data");
                                        var text = GetString(state, "Text");

                                        _logger.LogWarning(
                                            "Status state for {Symbol}: Stream={Stream}, Data={Data}, Text={Text}",
                                            symbol,
                                            stream,
                                            data,
                                            text);
                                    }

                                    snapshotReceived = true;
                                    break;
                                }
                                else if (string.Equals(type, "Error", StringComparison.OrdinalIgnoreCase))
                                {
                                    var text = GetString(message, "Text");
                                    _logger.LogError("Reuters ERROR received for {Symbol} (ID={RequestId}): {Text}", symbol, requestId, text);

                                    snapshotReceived = true;
                                    break;
                                }
                            }
                        }

                        if (!snapshotReceived)
                        {
                            _logger.LogWarning("No Reuters snapshot received for {Symbol}.", symbol);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error receiving Reuters snapshot for symbol {Symbol}. Skipping.", symbol);
                    }
                }

                _logger.LogInformation(
                    "Reuters EOD import complete. Total records collected: {Count}.",
                    results.Count);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "Reuters WebSocket error.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Reuters JSON error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while communicating with Reuters.");
            }
            finally
            {
                try
                {
                    if (socket.State == System.Net.WebSockets.WebSocketState.Open)
                    {
                        using var closeCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                        await socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "EOD import complete",
                            closeCts.Token);
                    }
                }
                catch
                {
                    // Ignore close errors.
                }
            }

            return results;
        }

        // =============================================================
        // PARSE MESSAGE ELEMENTS (ARRAY OR SINGLE OBJECT)
        // =============================================================

        private static List<JsonElement> ParseMessageElements(JsonDocument document)
        {
            var elements = new List<JsonElement>();
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in root.EnumerateArray())
                {
                    elements.Add(item);
                }
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                elements.Add(root);
            }

            return elements;
        }

        // =============================================================
        // PROCESS PING MESSAGES AND RESPOND WITH PONG
        // =============================================================

        private static async Task ProcessPingMessagesAsync(ClientWebSocket socket, IEnumerable<JsonElement> messages, ILogger logger)
        {
            foreach (var message in messages)
            {
                var type = GetString(message, "Type");
                if (string.Equals(type, "Ping", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogInformation("Received Ping from Reuters. Sending Pong...");

                    var pong = new { Type = "Pong" };
                    await SendJsonAsync(socket, pong);
                }
            }
        }

        // =============================================================
        // SEND JSON
        // =============================================================

        private static async Task SendJsonAsync(ClientWebSocket socket, object message)
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            using var sendCts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                sendCts.Token);
        }

        // =============================================================
        // RECEIVE COMPLETE WEBSOCKET MESSAGE
        // =============================================================

        private static async Task<string> ReceiveMessageAsync(ClientWebSocket socket, CancellationToken ct = default)
        {
            const long maxMessageSize = 5 * 1024 * 1024; // 5 MB max
            var buffer = new byte[8192];
            using var stream = new MemoryStream();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cts.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    throw new WebSocketException("Reuters closed the WebSocket connection.");
                }

                if (stream.Length + result.Count > maxMessageSize)
                {
                    throw new InvalidOperationException($"Reuters WebSocket message exceeded maximum allowed size ({maxMessageSize} bytes).");
                }

                stream.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        // =============================================================
        // GET JSON STRING PROPERTY
        // =============================================================

        private static string? GetString(JsonElement element, string property)
        {
            if (!element.TryGetProperty(property, out var value))
                return null;

            return value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : value.ToString();
        }

        private static string GetLocalHostIp()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch
            {
                // Fallback to standard localhost loopback if host resolution is restricted
            }
            return "127.0.0.1";
        }
    }
}