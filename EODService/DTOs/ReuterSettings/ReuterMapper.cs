using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EODService.DTOs.ReuterSettings
{
    public static class ReuterMapper
    {
        private static readonly string[] DateFormats = new[]
        {
            "yyyy-MM-dd",
            "dd MMM yyyy",
            "d MMM yyyy",
            "dd-MMM-yyyy",
            "d-MMM-yyyy",
            "dd/MM/yyyy",
            "d/M/yyyy",
            "MM/dd/yyyy",
            "yyyyMMdd",
            "yyyy/MM/dd",
            "dd.MM.yyyy"
        };

        public static EodData? Map(
            WebSocketResponse response,
            int Id,
            string Name,
            Action<string>? logWarning = null)
        {
            // Validate response
            if (response == null || response.Fields == null)
            {
                logWarning?.Invoke($"Response or Fields object was null for ID={Id} Name={Name}");
                return null;
            }

            var fields = response.Fields;

            // Resolve Close price:
            // 1. OFF_CLOSE: Official closing auction price (available after market close)
            // 2. TRDPRC_1: Last traded transaction price of the day
            // 3. HST_CLOSE / ADJUST_CLS: Previous close as safety fallback
            decimal? effectiveClose = fields.Close ?? fields.LastPrice ?? fields.HstClose ?? fields.AdjustedClose;

            // Validate that we at least have a valid Close/Price
            if (effectiveClose == null || effectiveClose.Value <= 0)
            {
                logWarning?.Invoke($"No valid Close price (OFF_CLOSE, HST_CLOSE, TRDPRC_1) for ID={Id} Name={Name}");
                return null;
            }

            // Parse date
            if (string.IsNullOrWhiteSpace(fields.TradeDate))
            {
                logWarning?.Invoke($"TRADE_DATE is missing for ID={Id} Name={Name}. Skipping.");
                return null;
            }

            if (!DateTime.TryParseExact(
                    fields.TradeDate.Trim(),
                    DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out DateTime parsedDate) &&
                !DateTime.TryParse(
                    fields.TradeDate.Trim(),
                    new CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out parsedDate))
            {
                logWarning?.Invoke($"Could not parse TRADE_DATE '{fields.TradeDate}' for ID={Id} Name={Name}. Rejecting record.");
                return null;
            }

            decimal parsedClose = effectiveClose.Value;

            // Financial Sanity Checks
            if (fields.High.HasValue && fields.Low.HasValue && fields.High.Value < fields.Low.Value)
            {
                logWarning?.Invoke($"HIGH_1 ({fields.High.Value}) < LOW_1 ({fields.Low.Value}) for ID={Id} Name={Name}");
                return null;
            }

            if (fields.Open.HasValue && fields.Open.Value < 0)
            {
                logWarning?.Invoke($"OPEN_PRC ({fields.Open.Value}) < 0 for ID={Id} Name={Name}");
                return null;
            }

            if (fields.Volume.HasValue && fields.Volume.Value < 0)
            {
                logWarning?.Invoke($"ACVOL_1 ({fields.Volume.Value}) < 0 for ID={Id} Name={Name}");
                return null;
            }

            return new EodData
            {
                Id = Id,
                Name = Name,
                Date = parsedDate.Date,

                // Keep real values as returned by Reuters (can be null if untraded)
                Open = fields.Open,
                High = fields.High,
                Low = fields.Low,
                Close = parsedClose,

                AdjustedClose = fields.AdjustedClose ?? parsedClose,
                Volume = fields.Volume ?? 0
            };
        }
    }
}
