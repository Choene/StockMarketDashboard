using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMarketDashboard.Models;
using StockMarketDashboard.Services;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;
    private readonly ILogger<StockController> _logger;

    public StockController(StockService stockService, ILogger<StockController> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    // GET /api/stocks/{symbol}
    [HttpGet("{symbol}")]
    public async Task<IActionResult> GetStockData(string symbol)
    {
        // Log the authenticated user
        var username = User.Identity?.Name;
        _logger.LogInformation($"User {username} requested stock data for {symbol}");

        try
        {
            var stockData = await _stockService.GetStockDataAsync(symbol.ToUpper());
            return Ok(stockData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // GET /api/stocks
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllStockData()
    {
        var symbols = new[] { "MSFT", "AAPL", "NFLX", "META", "AMZN" };
        var tasks = symbols.Select(async symbol =>
        {
            try
            {
                var stockData = await _stockService.GetStockDataAsync(symbol);
                return (Success: true, Symbol: symbol, Data: stockData, Error: (string)null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data for {symbol}: {ex.Message}");
                return (Success: false, Symbol: symbol, Data: (StockResponse)null, Error: ex.Message);
            }
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        var stockDataList = results.Where(result => result.Success).Select(result => result.Data).ToList();
        var failedSymbols = results.Where(result => !result.Success).Select(result => result.Symbol).ToList();

        if (stockDataList.Count == 0)
        {
            return StatusCode(500, "Failed to fetch data for all symbols.");
        }

        if (failedSymbols.Count > 0)
        {
            return Ok(new
            {
                Data = stockDataList,
                Warning = $"Failed to fetch data for the following symbols: {string.Join(", ", failedSymbols)}"
            });
        }

        return Ok(stockDataList);
    }
}
