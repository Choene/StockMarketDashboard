namespace StockMarketDashboard.Models
{
    public class MetaData
    {
        public string ?Information { get; set; }
        public string ?Symbol { get; set; }
        public string ?LastRefreshed { get; set; }
        public string ?OutputSize { get; set; }
        public string ?TimeZone { get; set; }
    }

    public class StockResponse
    {
        public MetaData ?MetaData { get; set; }
        public Dictionary<string, Dictionary<string, string>> ?TimeSeriesDaily { get; set; }
    }
}
