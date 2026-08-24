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

                await socket.ConnectAsync(new Uri(wsUri), CancellationToken.None);

                _logger.LogInformation("WebSocket connected.");

                // =====================================================
                // 2. LOGIN
                // =====================================================

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
                            Position = Position                        // hardcoded
                        }
                    }
                };

                await SendJsonAsync(socket, loginRequest);

                _logger.LogInformation(
                    "LOGIN REQUEST: {Json}",
                    JsonSerializer.Serialize(loginRequest));

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

                    foreach (var message in messages)
                    {
                        var domain = GetString(message, "Domain");
                        var type = GetString(message, "Type");

                        if (domain == "Login" && string.Equals(type, "Refresh", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("LOGIN RESPONSE RECEIVED.");

                            if (message.TryGetProperty("State", out var state))
                            {
                                var data = GetString(state, "Data");

                                _logger.LogInformation("Login state: {State}", data);

                                if (string.Equals(data, "Ok", StringComparison.OrdinalIgnoreCase))
                                {
                                    loginSuccessful = true;
                                }
                            }

                            break;
                        }
                        else if (domain == "Login" && string.Equals(type, "Status", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("LOGIN STATUS RECEIVED.");

                            if (message.TryGetProperty("State", out var state))
                            {
                                var data = GetString(state, "Data");
                                var text = GetString(state, "Text");

                                _logger.LogWarning("Login status state: Data={Data}, Text={Text}", data, text);

                                if (!string.Equals(data, "Ok", StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                        else if (string.Equals(type, "Error", StringComparison.OrdinalIgnoreCase))
                        {
                            var text = GetString(message, "Text");
                            _logger.LogError("Reuters LOGIN ERROR: {Text}", text);
                            break;
                        }
                    }

                    if (loginSuccessful)
                        break;
                }

                // =====================================================
                // 4. CHECK LOGIN
                // =====================================================

                if (!loginSuccessful)
                {
                    _logger.LogError("Reuters login was not successful.");
                    return results;
                }

                _logger.LogInformation("Reuters login successful.");

                // =====================================================
                // 5. REQUEST SNAPSHOT FOR EACH SYMBOL
                // =====================================================

                for (int i = 0; i < _symbolSettings.Symbols.Count; i++)
                {
                    var symbol = _symbolSettings.Symbols[i];   // from DB
                    var id = _symbolSettings.Ids[i];           // from DB
                    var name = _symbolSettings.Names[i];       // from DB

                    int requestId = i + 2;

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

                    _logger.LogInformation(
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
                                _logger.LogInformation(
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

                                    _logger.LogInformation(
                                        "Successfully mapped Reuters EOD data for {Symbol}: Date={Date:yyyy-MM-dd}, Open={Open}, High={High}, Low={Low}, Close={Close}, Vol={Volume}",
                                        symbol,
                                        eodData.Date,
                                        eodData.Open,
                                        eodData.High,
                                        eodData.Low,
                                        eodData.Close,
                                        eodData.Volume);
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
                        await socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "EOD import complete",
                            CancellationToken.None);
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

            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        // =============================================================
        // RECEIVE COMPLETE WEBSOCKET MESSAGE
        // =============================================================

        private static async Task<string> ReceiveMessageAsync(ClientWebSocket socket)
        {
            var buffer = new byte[8192];

            using var stream = new MemoryStream();

            WebSocketReceiveResult result;

            do
            {
                result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    throw new WebSocketException("Reuters closed the WebSocket connection.");
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
    }
}