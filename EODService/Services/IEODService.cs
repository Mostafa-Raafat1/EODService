using EODService.DTOs.EOD;

namespace EODService.Services
{
    /// <summary>
    /// Defines the contract for an End-of-Day stock data import service.
    /// </summary>
    public interface IEODService
    {
        /// <summary>
        /// Downloads EOD price data for all configured symbols and returns the results.
        /// </summary>
        /// <returns>A list of EOD records across all symbols.</returns>
        Task<List<EodData>> GetEodDataAsync();
    }
}
