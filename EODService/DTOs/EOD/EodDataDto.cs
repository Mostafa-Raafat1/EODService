namespace EODService.DTOs.EOD
{
   
    public class EodDataDto
    {
        public string Symbol { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double? Open { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public double? Close { get; set; }
        public double? AdjClose { get; set; }
        public long? Volume { get; set; }
    }
}
