using EODService.DTOs.Stock;
using EODService.DTOs.SymbolSettings;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EODService.Persistance.Repo
{
    public interface IStock
    {
        Task<SymbolSettings> GetSymbolAndTickerIDAsync(
                Expression<Func<Stock, bool>> existsCondition,
                Func<Stock, string?> tickerSelector);
    }
}
