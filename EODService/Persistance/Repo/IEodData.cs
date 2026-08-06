using EODService.DTOs.EOD;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Persistance.Repo
{
    public interface IEodData
    {
        public Task<DateTime?> GetLastDateForSymbol(string symbol);
        public Task AddToHistoryAsync(EodDataHistory data);
    }
}