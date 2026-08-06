using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.EOD
{
    public static class EodMapper
    {

        // Adding mappers to EodData Class
        public static EodDataDaily ToDaily(this EodData dto)
        {
            return new EodDataDaily
            {
                Symbol = dto.Symbol,
                Date = dto.Date,
                Open = dto.Open,
                High = dto.High,
                Low = dto.Low,
                Close = dto.Close,
                AdjustedClose = dto.AdjustedClose,
                Volume = dto.Volume
            };
        }

        public static EodDataHistory ToHistory(this EodData dto)
        {
            return new EodDataHistory
            {
                Symbol = dto.Symbol,
                Date = dto.Date,
                Open = dto.Open,
                High = dto.High,
                Low = dto.Low,
                Close = dto.Close,
                AdjustedClose = dto.AdjustedClose,
                Volume = dto.Volume
            };
        }
        public static List<EodDataDaily> ToDaily(this IEnumerable<EodData> data)
        {
            return data.Select(x => new EodDataDaily
            {
                Symbol = x.Symbol,
                Date = x.Date,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                Close = x.Close,
                AdjustedClose = x.AdjustedClose,
                Volume = x.Volume
            }).ToList();
        }

        public static List<EodDataHistory> ToHistory(this IEnumerable<EodData> data)
        {
            return data.Select(x => new EodDataHistory
            {
                Symbol = x.Symbol,
                Date = x.Date,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                Close = x.Close,
                AdjustedClose = x.AdjustedClose,
                Volume = x.Volume
            }).ToList();
        }
    }
}
