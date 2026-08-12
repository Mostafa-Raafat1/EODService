using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EODService.Persistance.Repo
{
    public interface IEodData
    {
        Task<DateTime?> GetLastDateForIdAsync(int Id);
        Task AddToHistoryAsync(EodDataHistory data);
        Task SaveBatchAsync(IEnumerable<EodData> data);
    }
}