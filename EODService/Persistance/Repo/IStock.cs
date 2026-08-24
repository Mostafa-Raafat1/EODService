using EODService.DTOs.SymbolSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Persistance.Repo
{
    public interface IStock
    {
        public Task<SymbolSettings> GetSymbolAndTickerIDForYahooFinance();
        public Task<SymbolSettings> GetSymbolAndTickerIDForTwelveData();
    }
}
