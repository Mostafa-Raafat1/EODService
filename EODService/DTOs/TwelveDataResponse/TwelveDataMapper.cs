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

            // Find the latest value that can be successfully parsed and passes sanity checks
            foreach (var v in response.Values)
            {
                if (v.Datetime == null || v.Open == null || v.High == null || v.Low == null || v.Close == null || v.Volume == null)
                    continue;

                // Try parsing safely
                if (!DateTime.TryParseExact(v.Datetime, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                    continue;

                if (!decimal.TryParse(v.Open, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedOpen) ||
                    !decimal.TryParse(v.High, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedHigh) ||
                    !decimal.TryParse(v.Low, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedLow) ||
                    !decimal.TryParse(v.Close, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedClose) ||
                    !long.TryParse(v.Volume, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedVolume))
                {
                    continue;
                }

                // Financial Sanity Checks
                if (parsedHigh < parsedLow || parsedOpen < 0 || parsedClose <= 0 || parsedVolume < 0)
                    continue;

                return new EodData
                {
                    Symbol = requestedSymbol,
                    Date = parsedDate,
                    Open = parsedOpen,
                    High = parsedHigh,
                    Low = parsedLow,
                    Close = parsedClose,
                    AdjustedClose = parsedClose, // Twelve Data basic endpoint doesn't return Adjusted Close
                    Volume = parsedVolume
                };
            }

            return null;
        }
    }
}
