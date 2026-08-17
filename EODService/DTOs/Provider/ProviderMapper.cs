using EODService.Models.Provider;
using System;
using System.Collections.Generic;
using System.Text;
namespace EODService.DTOs.Provider
{
    public static class ProviderMapper
    {
        public static ProviderDTO? Map(EODService.Models.Provider.Provider? provider)
        {
            if (provider == null)
                return null;

            return new ProviderDTO
            {
                Id = provider.Id,
                Name = provider.Name,
                BaseUrl = provider.BaseUrl,
                EndPoint = provider.EndPoint,
                ApiKey = provider.ApiKey,
                Parameters = provider.Parameters
            };
        }

    }
}
