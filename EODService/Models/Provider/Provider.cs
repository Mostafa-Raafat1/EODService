using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Models.Provider
{
    public class Provider
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string EndPoint { get; set; }
        public string? ApiKey { get; set; }

    }
}
