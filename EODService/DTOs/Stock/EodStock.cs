using System;

namespace EODService.DTOs.Stock
{
    public class EodStock
    {
        public int Id { get; set; }
        public string StockName { get; set; } = string.Empty;
        public string? InitialId { get; set; }
        public string? Exchange { get; set; }
        public bool TdTradable { get; set; } = true;
        public bool YfTradable { get; set; } = true;
        public string? TdSymbol { get; set; }
        public string? YfSymbol { get; set; }
    }
}
