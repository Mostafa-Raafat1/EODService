using EODService.DTOs.EOD;
using System.Globalization;

namespace EODService.DTOs.TwelveDataResponse
{
    public static class TwelveDataMapper
    {
        public static EodData? Map(TwelveDataResponse response, string requestedSymbol)
        {
            // Validate the response status and that values exist
            if (response == null || response.Status != "ok" || response.Values == null || !response.Values.Any())
                return null;

            // Find the latest value that has all required fields populated
            // (same defensive approach as YahooEoadMapper)
            var latestValue = response.Values
                .FirstOrDefault(v =>
                    v.Datetime != null &&
                    v.Open     != null &&
                    v.High     != null &&
                    v.Low      != null &&
                    v.Close    != null &&
                    v.Volume   != null);

            if (latestValue == null)
                return null;

            return new EodData
            {
                Symbol = requestedSymbol,

                // Twelve Data returns "YYYY-MM-DD" for daily interval
                Date = DateTime.ParseExact(
                    latestValue.Datetime!,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture),

                Open   = decimal.Parse(latestValue.Open!,   CultureInfo.InvariantCulture),
                High   = decimal.Parse(latestValue.High!,   CultureInfo.InvariantCulture),
                Low    = decimal.Parse(latestValue.Low!,    CultureInfo.InvariantCulture),
                Close  = decimal.Parse(latestValue.Close!,  CultureInfo.InvariantCulture),

                // Twelve Data basic endpoint doesn't return Adjusted Close.
                // We use Close as the adjusted value to keep the model consistent.
                AdjustedClose = decimal.Parse(latestValue.Close!, CultureInfo.InvariantCulture),

                Volume = long.Parse(latestValue.Volume!, CultureInfo.InvariantCulture)
            };
        }
    }
}
