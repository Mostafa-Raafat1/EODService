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

            return new SymbolSettings
            {
                Symbols = stocks
                    .Select(tickerSelector)
                    .Where(ticker => ticker != null)
                    .Select(ticker => ticker!)
                    .ToList(),

                Ids = stocks
                    .Select(s => s.Id)
                    .ToList(),

                Names = stocks
                    .Select(s => s.StockName)
                    .ToList()
            };
        }
    }
}
