using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.SymbolSettings
{
    public class SymbolSettings
    {
        public List<string> Symbols { get; set; } = new();
        public List<string> TickerID { get; set; } = new();
    }
}
