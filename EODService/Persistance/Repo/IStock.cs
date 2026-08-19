using EODService.DTOs.SymbolSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Persistance.Repo
{
    public interface IStock
    {
        Task<SymbolSettings> GetSymbolsByProviderIdAsync(int providerId);
        Task<SymbolSettings> GetSymbolAndTickerIDForYahooFinance();
        Task<SymbolSettings> GetSymbolAndTickerIDForTwelveData();
    }
}
