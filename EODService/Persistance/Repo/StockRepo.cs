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
        // delegegate func here will be implemented in the future for more generic code, but for now we will have two separate methods for YahooFinance and TwelveData
        public async Task<SymbolSettings> GetSymbolAndTickerIDForYahooFinance()
        {
            var stocks = await dbContext.Stock
                .Where(s => s.YahooFinanceExists &&
                            s.YahooFinanceID != null)
                .ToListAsync();

            return new SymbolSettings
            {
                Symbols = stocks
                    .Select(s => s.YahooFinanceID!)
                    .ToList(),

                Ids = stocks
                    .Select(s => s.Id)
                    .ToList(),
                Names = stocks
                    .Select(s => s.StockName)
                    .ToList()

            };
        }

        public async Task<SymbolSettings> GetSymbolAndTickerIDForTwelveData()
        {
            var stocks = await dbContext.Stock
                .Where(s => s.TwelveDataExists &&
                            s.TwelveDataID != null)
                .ToListAsync();

            return new SymbolSettings
            {
                Symbols = stocks
                    .Select(s => s.TwelveDataID!)
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
