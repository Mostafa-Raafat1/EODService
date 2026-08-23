using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EODService.DTOs.ReuterSettings
{
    public static class ReuterMapper
    {
        public static EodData? Map(
            WebSocketResponse response,
            int Id,
            string Name)
        {
            // Validate response
            if (response == null ||
                response.Fields == null)
            {
                return null;
            }

            var fields = response.Fields;

            // Validate required fields
            if (fields.TradeDate == null ||
                fields.Open == null ||
                fields.High == null ||
                fields.Low == null ||
                fields.Close == null ||
                fields.Volume == null)
            {
                return null;
            }

            // Parse date
            if (!DateTime.TryParseExact(
                    fields.TradeDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedDate))
            {
                return null;
            }

            decimal parsedOpen = fields.Open.Value;
            decimal parsedHigh = fields.High.Value;
            decimal parsedLow = fields.Low.Value;
            decimal parsedClose = fields.Close.Value;
            long parsedVolume = fields.Volume.Value;

            // Financial sanity checks
            if (parsedHigh < parsedLow ||
                parsedOpen < 0 ||
                parsedClose <= 0 ||
                parsedVolume < 0)
            {
                return null;
            }

            return new EodData
            {
                Id = Id,
                Name = Name,
                Date = parsedDate.Date,

                Open = parsedOpen,
                High = parsedHigh,
                Low = parsedLow,
                Close = parsedClose,

                AdjustedClose = fields.AdjustedClose ?? parsedClose,

                Volume = parsedVolume
            };
        }
    }
}
