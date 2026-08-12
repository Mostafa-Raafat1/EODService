namespace EODService.DTOs.TwelveDataSettings
{
    public class TwelveDataSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public int OutputSize { get; set; }
        public string ApiKey { get; set; } = string.Empty;
    }
}
