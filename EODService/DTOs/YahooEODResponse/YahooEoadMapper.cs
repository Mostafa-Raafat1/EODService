using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EODService.DTOs.YahooEODResponse
{
    public static class YahooEodMapper
    {
        public static EodData? Map(YahooEodResponse response, int Id, string Name)
        {
            // get Result Object
            var result = response.Chart?.Result?.FirstOrDefault();

            // Check if result is null
            if (result == null) { return null; }

            // Check if timestamps and indicators are null
            if (result.Timestamp == null || result.Indicators?.Quote == null || !result.Indicators.Quote.Any())
            {
                return null;
            }

            // get Quote
            var quote = result.Indicators.Quote.First();

            // Check if Quote or Close list is null
            if (quote == null || quote.Close == null) { return null; }

            var timestamps = result.Timestamp;

            // Find the latest valid EOD record
            for (int i = timestamps.Count - 1; i >= 0; i--)
            {
                if (i >= quote.Close.Count || quote.Close[i] is null)
                    continue;

                var c = GetValue(quote.Close, i);
                var o = GetValue(quote.Open, i);
                var h = GetValue(quote.High, i);
                var l = GetValue(quote.Low, i);
                var v = GetValue(quote.Volume, i);

                // Financial Sanity Checks (allow nulls for some fields if Yahoo omits them, but validate if present)
                if (c <= 0) continue;
                if (h.HasValue && l.HasValue && h < l) continue;
                if (o.HasValue && o < 0) continue;
                if (v.HasValue && v < 0) continue;

                // Map All the data to EodData object and return it
                return new EodData
                {
                    Id = Id,
                    Name = Name,
                    Date = DateTimeOffset
                        .FromUnixTimeSeconds(timestamps[i])
                        .UtcDateTime,
                    Open = o,
                    High = h,
                    Low = l,
                    Close = c,
                    Volume = v,
                    AdjustedClose = GetValue(
                        result.Indicators.Adjclose?
                            .FirstOrDefault()?
                            .Adjclose,
                        i)
                };
            }

            return null;
        }

        private static T? GetValue<T>(List<T?>? values, int index) where T : struct
        {
            if (values is null || index >= values.Count)
                return null;

            return values[index];
        }
    }
}
