using EODService.DTOs.EOD;

namespace EODService.Services
{
    
    public interface IEODService
    {
       
        Task<List<EodData>> GetEodDataAsync();
    }
}
