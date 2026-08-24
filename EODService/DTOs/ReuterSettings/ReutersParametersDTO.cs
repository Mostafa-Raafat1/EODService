using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.ReuterSettings
{
    public class ReutersParametersDTO
    {
        public string DacsUser { get; set; } = string.Empty;

        public int ServiceId { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public string ApplicationId { get; set; } = string.Empty;
    }
}
