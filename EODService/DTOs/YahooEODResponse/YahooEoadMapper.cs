using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EODService.DTOs.YahooEODResponse
{
    public static class YahooEoadMapper
    {
        public static EodData? Map(YahooEodResponse response, string symbol)
        {
            // get Result Object
            var result = response.Chart?.Result?.FirstOrDefault();

            // Check if result is null
            if (result == null) { return null; }

            // Check if timestamps and indicators are null
            if (result.Timestamp == null || result.Indicators?.Quote == null || result.Indicators.Quote.Count == 0)
            {
                return null;
            }


            // get Qoute
            var quote = result.Indicators.Quote.First();

            //Check if Close is null or empty
            if (quote == null) { return null; };


            var timestamps = result.Timestamp;

            // Find the latest valid EOD record
            for (int i = timestamps.Count - 1; i >= 0; i--)
            {
                if (i >= quote.Close.Count || quote.Close[i] is null)
                    continue;



                // Mapp All the data to EodData object and return it
                return new EodData
                {
                    Symbol = symbol,

                    Date = DateTimeOffset
                   .FromUnixTimeSeconds(timestamps[i])
                   .UtcDateTime,

                    Open = GetValue(quote.Open, i),
                    High = GetValue(quote.High, i),
                    Low = GetValue(quote.Low, i),
                    Close = GetValue(quote.Close, i),
                    Volume = GetValue(quote.Volume, i),

                    AdjustedClose = GetValue(
                    result.Indicators.Adjclose?
                       .FirstOrDefault()?
                       .Adjclose,
                   i)
                };

            }

            return null;

        }

        private static T? GetValue<T>(
                        List<T?>? values,
                        int index)
        where T : struct
        {
            if (values is null || index >= values.Count)
                return null;

            return values[index];
        }
    }
}
