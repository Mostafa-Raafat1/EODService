using EODService.DTOs.EOD;

namespace EODService.Services
{
    
    public interface IEODService
    {
      
        Task<List<EodDataDto>> GetEodDataAsync();
    }
}
