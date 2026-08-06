using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EODService.Persistance.Repo
{
    public class EodDataRepo : IEodData
    {
        private readonly AppDbContext appDb;

        public EodDataRepo(AppDbContext appDb)
        {
            this.appDb = appDb;
        }

        public async Task AddToHistoryAsync(EodDataHistory data)
        {
            await appDb.EodHistory.AddAsync(new EodDataHistory
            {
                Symbol = data.Symbol,
                Date = data.Date,
                Open = data.Open,
                High = data.High,
                Low = data.Low,
                Close = data.Close,
                Volume = data.Volume
            });
        }

        public async Task<DateTime?> GetLastDateForSymbol(string symbol)
        {
            return await appDb.EodDaily
                .Where(x => x.Symbol == symbol)
                .MaxAsync(x => (DateTime?)x.Date);
        }
    }
}
