using EODService.DTOs.EOD;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EODService.Persistance.Repo
{
    public class EodDataRepo : IEodData
    {
        private readonly AppDbContext _appDb;

        public EodDataRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task AddToHistoryAsync(EodDataHistory data)
        {
            await _appDb.EodHistory.AddAsync(data);
            await _appDb.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastDateForSymbol(string symbol)
        {
            return await _appDb.EodHistory
                .Where(x => x.Symbol == symbol)
                .MaxAsync(x => (DateTime?)x.Date);
        }
    }
}
