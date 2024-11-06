using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StockMarketDashboard.Models;
using System.Globalization;

namespace StockMarketDashboard.Services
{
    public class StockService
    {
        private readonly IDistributedCache _redisCache;
        private readonly IMemoryCache _memoryCache;
        private readonly StockApiConfig _config;
        private readonly HttpClient _httpClient;

        public StockService(IDistributedCache redisCache, IMemoryCache memoryCache, IOptions<StockApiConfig> config)
        {
            _redisCache = redisCache;
            _memoryCache = memoryCache;
            _config = config.Value;
            _httpClient = new HttpClient();
        }

        public async Task<StockResponse> GetStockDataAsync(string symbol)
        {
            string cacheKey = $"StockData_{symbol}";

            // Try to get data from Redis with a timeout of 3 seconds
            var redisDataTask = _redisCache.GetStringAsync(cacheKey);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3));

            var completedTask = await Task.WhenAny(redisDataTask, timeoutTask);
            if (completedTask == redisDataTask && !string.IsNullOrEmpty(await redisDataTask))
            {
                // If Redis returned data within 3 seconds
                Console.WriteLine("Redis cache hit.");
                var cachedData = await redisDataTask;
                return JsonSerializer.Deserialize<StockResponse>(cachedData);
            }
            else
            {
                Console.WriteLine("Redis cache miss or timeout. Checking in-memory cache.");
            }

            // Try to get data from in-memory cache if Redis is not available or took too long
            if (_memoryCache.TryGetValue(cacheKey, out StockResponse memoryCachedData))
            {
                Console.WriteLine("In-memory cache hit.");
                return memoryCachedData;
            }

            // Fetch data from API if neither cache is available
            Console.WriteLine("Fetching from API...");
            var requestUrl = $"{_config.BaseUrl}&symbol={symbol}&apikey={_config.ApiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("X-RapidAPI-Host", _config.Host);
            request.Headers.Add("X-RapidAPI-Key", _config.ApiKey);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var stockData = ParseStockData(json, symbol);

                // Cache in both Redis and in-memory with expiration
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_config.CacheDurationInMinutes)
                };
                var serializedData = JsonSerializer.Serialize(stockData);

                // Save to Redis cache
                await _redisCache.SetStringAsync(cacheKey, serializedData, cacheOptions);

                // Save to in-memory cache
                _memoryCache.Set(cacheKey, stockData, TimeSpan.FromMinutes(_config.CacheDurationInMinutes));

                return stockData;
            }

            throw new Exception("Failed to fetch stock data from the API.");
        }

        private static StockResponse ParseStockData(string json, string symbol)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("Meta Data", out var meta) || !root.TryGetProperty("Time Series (Daily)", out var timeSeries))
            {
                throw new KeyNotFoundException("Required data is missing in the response.");
            }

            var metaData = new MetaData
            {
                Information = meta.GetProperty("1. Information").GetString() ?? string.Empty,
                Symbol = meta.GetProperty("2. Symbol").GetString() ?? string.Empty,
                LastRefreshed = meta.GetProperty("3. Last Refreshed").GetString() ?? string.Empty,
                OutputSize = meta.GetProperty("4. Output Size").GetString() ?? string.Empty,
                TimeZone = meta.GetProperty("5. Time Zone").GetString() ?? string.Empty
            };

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

            return new StockResponse
            {
                MetaData = metaData,
                TimeSeriesDaily = timeSeriesDaily
            };
        }
    }
}
