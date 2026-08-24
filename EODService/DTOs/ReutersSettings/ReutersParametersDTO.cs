using System;

namespace EODService.DTOs.ReutersSettings
{
    public class ReutersParametersDTO
    {
        public string DacsUser { get; set; } = "EODService";
        public int ServiceId { get; set; } = 27;
        public string ServiceName { get; set; } = "WWEIKON";
        public string ApplicationId { get; set; } = "256";
    }
}
