using EODService.DTOs.SymbolSettings;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EODService.Persistance.Repo
{
    public class StockRepo : IStock
    {
        private readonly AppDbContext dbContext;

        public StockRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<SymbolSettings> GetSymbolAndTickerIDForYahooFinance()
        {
            var stocks = await dbContext.Stock
                .Where(s => s.YahooFinanceExists &&
                            s.YahooFinanceID != null &&
                            s.TickerID != null)
                .ToListAsync();

            return new SymbolSettings
            {
                Symbols = stocks
                    .Select(s => s.YahooFinanceID!)
                    .ToList(),

                TickerID = stocks
                    .Select(s => s.TickerID!)
                    .ToList()
            };
        }

        public async Task<SymbolSettings> GetSymbolAndTickerIDForTwelveData()
        {
            var stocks = await dbContext.Stock
                .Where(s => s.TwelveDataExists &&
                            s.TwelveDataID != null &&
                            s.TickerID != null)
                .ToListAsync();

            return new SymbolSettings
            {
                Symbols = stocks
                    .Select(s => s.TwelveDataID!)
                    .ToList(),

                TickerID = stocks
                    .Select(s => s.TickerID!)
                    .ToList()
            };
        }
    }
}
