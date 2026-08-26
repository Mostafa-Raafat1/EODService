using EODService.DTOs.Stock;
using EODService.DTOs.SymbolSettings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EODService.Persistance.Repo
{
    public class StockRepo : IStock
    {
        private readonly AppDbContext _dbContext;

        public StockRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        // delegate function to get symbols based of the given column no redundant code
        public async Task<SymbolSettings> GetSymbolAndTickerIDAsync(
                                            Expression<Func<Stock, bool>> existsCondition,
                                            Func<Stock, string?> tickerSelector)
        {
            var stocks = await _dbContext.Stock
                .Where(existsCondition)
                .ToListAsync();

            // Filter out items with null or whitespace tickers first to guarantee 1:1 index alignment across Symbols, Ids, and Names
            var validStocks = stocks
                .Select(s => new { Stock = s, Ticker = tickerSelector(s)?.Trim() })
                .Where(x => !string.IsNullOrWhiteSpace(x.Ticker))
                .ToList();

            return new SymbolSettings
            {
                Symbols = validStocks.Select(x => x.Ticker!).ToList(),
                Ids     = validStocks.Select(x => x.Stock.Id).ToList(),
                Names   = validStocks.Select(x => x.Stock.StockName?.Trim() ?? string.Empty).ToList()
            };
        }
    }
}
