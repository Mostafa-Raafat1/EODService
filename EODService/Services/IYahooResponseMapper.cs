using EODService.DTOs.EOD;

namespace EODService.Services
{
   
    public interface IYahooResponseMapper
    {

        List<EodDataDto> Map(string jsonResponse, string symbol);
    }
}
