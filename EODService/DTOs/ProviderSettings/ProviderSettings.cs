namespace EODService.DTOs.ProviderSettings
{
    public class ProviderSettings
    {
        
        /// Accepted values:
        ///   "Yahoo"       — Yahoo Finance (https://finance.yahoo.com)
        ///   "TwelveData"  — Twelve Data   (https://twelvedata.com)
       
        public int ActiveProvider { get; set; } = 1;
    }
}
