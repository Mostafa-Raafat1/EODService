using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Models.Provider
{
    public class Provider
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public string? ApiKey { get; set; }
        public string Parameters { get; set; } = string.Empty;
    }
}
