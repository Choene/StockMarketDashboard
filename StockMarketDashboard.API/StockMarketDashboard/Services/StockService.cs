using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StockMarketDashboard.Models;
using System.Globalization;

namespace StockMarketDashboard.Services
{
    public class StockService
    {
        private readonly IMemoryCache _cache;
        private readonly StockApiConfig _config;
        private readonly HttpClient _httpClient;

        public StockService(IMemoryCache cache, IOptions<StockApiConfig> config)
        {
            _cache = cache;
            _config = config.Value;
            _httpClient = new HttpClient();
        }

        // Update return type to StockResponse
        public async Task<StockResponse> GetStockDataAsync(string symbol)
        {
            string cacheKey = $"StockData_{symbol}";

            // Check cache for StockResponse instead of StockData
            if (_cache.TryGetValue(cacheKey, out StockResponse cachedData))
                return cachedData;

            var requestUrl = $"{_config.BaseUrl}&symbol={symbol}&apikey={_config.ApiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("X-RapidAPI-Host", _config.Host);
            request.Headers.Add("X-RapidAPI-Key", _config.ApiKey);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Parse JSON to StockResponse
                var stockData = ParseStockData(json, symbol);

                // Cache the StockResponse object for minutes configured in CacheDurationInMinutes
                _cache.Set(cacheKey, stockData, TimeSpan.FromMinutes(_config.CacheDurationInMinutes));

                return stockData;
            }

            throw new Exception("Failed to fetch stock data.");
        }

        private static StockResponse ParseStockData(string json, string symbol)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Check if "Meta Data" and "Time Series (Daily)" keys are present
            if (!root.TryGetProperty("Meta Data", out var meta) || !root.TryGetProperty("Time Series (Daily)", out var timeSeries))
            {
                throw new KeyNotFoundException("Required data is missing in the response.");
            }

            // Populate MetaData
            // var meta = root.GetProperty("Meta Data");
            var metaData = new MetaData
            {
                Information = meta.GetProperty("1. Information").GetString() ?? string.Empty,
                Symbol = meta.GetProperty("2. Symbol").GetString() ?? string.Empty,
                LastRefreshed = meta.GetProperty("3. Last Refreshed").GetString() ?? string.Empty,
                OutputSize = meta.GetProperty("4. Output Size").GetString() ?? string.Empty,
                TimeZone = meta.GetProperty("5. Time Zone").GetString() ?? string.Empty
            };

            // Populate Time Series Daily
            // var timeSeries = root.GetProperty("Time Series (Daily)");
            var timeSeriesDaily = new Dictionary<string, Dictionary<string, string>>();

            foreach (var day in timeSeries.EnumerateObject())
            {
                var date = day.Name;
                var dailyData = day.Value;

                timeSeriesDaily[date] = new Dictionary<string, string>
                {
                    { "open", dailyData.TryGetProperty("1. open", out var open) ? open.GetString() : "N/A" },
                    { "high", dailyData.TryGetProperty("2. high", out var high) ? high.GetString() : "N/A" },
                    { "low", dailyData.TryGetProperty("3. low", out var low) ? low.GetString() : "N/A" },
                    { "close", dailyData.TryGetProperty("4. close", out var close) ? close.GetString() : "N/A" },
                    { "volume", dailyData.TryGetProperty("5. volume", out var volume) ? volume.GetString() : "N/A" }
                };
            }


            // Stock Response
            return new StockResponse
            {
                MetaData = metaData,
                TimeSeriesDaily = timeSeriesDaily
            };
        }
    }
}
