using EODService.DTOs.EOD;
using EODService.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EODService.Persistance.Repo
{
    public class EodDataRepo : IEodData
    {
        private readonly AppDbContext _appDb;

        public EodDataRepo(AppDbContext appDb)
        {
            _appDb = appDb ?? throw new ArgumentNullException(nameof(appDb));
        }

        public async Task AddToHistoryAsync(EodDataHistory data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            await _appDb.EodHistory.AddAsync(data);
            await _appDb.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastDateForIdAsync(int Id)
        {
            return await _appDb.EodHistory
                .Where(x => x.Id == Id)
                .MaxAsync(x => (DateTime?)x.Date);
        }

        public async Task SaveBatchAsync(IEnumerable<EodData> data)
        {
            await EodPersistenceService.SaveEodDataAsync(data, _appDb);
        }
    }
}
