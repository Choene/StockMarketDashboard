namespace StockMarketDashboard.Models
{
    public class StockApiConfig
    {
        public string ?BaseUrl { get; set; }
        public string ?Host { get; set; }
        public string ?ApiKey { get; set; }
        public int CacheDurationInMinutes { get; set; }
    }
}
