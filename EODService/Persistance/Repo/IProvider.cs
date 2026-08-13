using EODService.Models.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Persistance.Repo
{
    public interface IProvider
    {
        public Task UpdateProvider(int providerId, string name, string baseUrl, string endPoint, string? apiKey);
        public Task<Provider> GetProviderById(int providerId);
    }
}
