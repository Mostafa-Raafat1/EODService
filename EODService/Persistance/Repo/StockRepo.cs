using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EODService.DTOs.Stock;
using EODService.DTOs.SymbolSettings;
using EODService.Models;

namespace EODService.Persistance.Repo
{
    public class StockRepo : IStock
    {
        private readonly AppDbContext _dbContext;

        public StockRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<SymbolSettings> GetSymbolsByProviderIdAsync(int providerId)
        {
            return providerId switch
            {
                (int)ProviderIds.Yahoo => await GetSymbolsInternalAsync(
                    s => s.YahooFinanceExists && s.YahooFinanceID != null,
                    s => s.YahooFinanceID),

                (int)ProviderIds.TwelveData => await GetSymbolsInternalAsync(
                    s => s.TwelveDataExists && s.TwelveDataID != null,
                    s => s.TwelveDataID),

                _ => new SymbolSettings()
            };
        }

        public Task<SymbolSettings> GetSymbolAndTickerIDForYahooFinance() =>
            GetSymbolsByProviderIdAsync((int)ProviderIds.Yahoo);

        public Task<SymbolSettings> GetSymbolAndTickerIDForTwelveData() =>
            GetSymbolsByProviderIdAsync((int)ProviderIds.TwelveData);

        private async Task<SymbolSettings> GetSymbolsInternalAsync(
            Expression<Func<Stock, bool>> predicate,
            Func<Stock, string?> symbolSelector)
        {
            var stocks = await _dbContext.Stock
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();

            var validStocks = stocks
                .Where(s => !string.IsNullOrWhiteSpace(symbolSelector(s)))
                .ToList();

            return new SymbolSettings
            {
                Symbols = validStocks.Select(s => symbolSelector(s)!.Trim()).ToList(),
                Ids     = validStocks.Select(s => s.Id).ToList(),
                Names   = validStocks.Select(s => s.StockName).ToList()
            };
        }
    }
}
