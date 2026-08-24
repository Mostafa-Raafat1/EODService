using EODService.DTOs.EOD;
using EODService.DTOs.Provider;
using EODService.DTOs.ReutersSettings;
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

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ReutersEODService(
            ProviderDTO providerSettings,
            SymbolSettings symbolSettings,
            ILogger<ReutersEODService> logger)
        {
            _providerSettings = providerSettings;
            _symbolSettings = symbolSettings;
            _logger = logger;

            _parameters = JsonSerializer.Deserialize<ReutersParametersDTO>(
                providerSettings.Parameters ?? "{}",
                _jsonOptions) ?? new ReutersParametersDTO();
        }

        public async Task<List<EodData>> GetEodDataAsync()
        {
            var results = new List<EodData>();
            var wsUri = $"{_providerSettings.BaseUrl}{_providerSettings.EndPoint}";

            _logger.LogInformation("Connecting to Reuters WebSocket at {Uri}...", wsUri);

            using var ws = new ClientWebSocket();
            ws.Options.AddSubProtocol("tr_json2");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                await ws.ConnectAsync(new Uri(wsUri), cts.Token);
                _logger.LogInformation("Connected to Reuters WebSocket server successfully.");

                // 1. Send Login Message
                var localIp = GetLocalIpAddress();
                var loginMsg = new
                {
                    ID = 1,
                    Domain = "Login",
                    Key = new
                    {
                        Name = _parameters.DacsUser,
                        Elements = new
                        {
                            ApplicationId = _parameters.ApplicationId,
                            Position = $"{localIp}/net"
                        }
                    }
                };

                var loginJson = JsonSerializer.Serialize(loginMsg);
                _logger.LogInformation("Sending Reuters Login for user '{User}' (Position: {Pos})...", _parameters.DacsUser, $"{localIp}/net");
                await SendTextAsync(ws, loginJson);

                var loginResponse = await ReadTextAsync(ws);
                _logger.LogInformation("Reuters Login Response: {Response}", loginResponse);

                if (!loginResponse.Contains("\"Stream\":\"Open\""))
                {
                    _logger.LogError("Reuters login was rejected. Response: {Response}", loginResponse);
                    return results;
                }

                _logger.LogInformation("Reuters Login accepted successfully!");

                // Connection established and verified
                _logger.LogInformation("Phase 1 connection test complete. Ready for data mapping phase.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while communicating with Reuters WebSocket server at {Uri}.", wsUri);
            }
            finally
            {
                if (ws.State == WebSocketState.Open)
                {
                    try
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    catch { }
                }
            }

            return results;
        }

        private static async Task SendTextAsync(ClientWebSocket ws, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static async Task<string> ReadTextAsync(ClientWebSocket ws)
        {
            var buffer = new byte[32768];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        private static string GetLocalIpAddress()
        {
            try
            {
                var hostName = Dns.GetHostName();
                var ipEntry = Dns.GetHostEntry(hostName);
                foreach (var ip in ipEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }

            return "127.0.0.1";
        }
    }
}
