using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.Provider
{
    public class ProviderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string EndPoint { get; set; }
        public string? ApiKey { get; set; }
        public string Parameters { get; set; }
    }
}
