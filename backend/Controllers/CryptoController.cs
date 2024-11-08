using CryptoArbitrageAPI.Models;
using CryptoArbitrageAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoArbitrageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase
    {
        private readonly ICoinGeckoService _coinGeckoService;
        private readonly IArbitrageService _arbitrageService;

        public CryptoController(ICoinGeckoService coinGeckoService, IArbitrageService arbitrageService)
        {
            _coinGeckoService = coinGeckoService;
            _arbitrageService = arbitrageService;
        }

        [HttpGet("cryptocurrencies")]
        public async Task<IActionResult> GetCryptocurrencies([FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            try
            {
                var cryptocurrencies = await _coinGeckoService.GetCryptocurrenciesByMarketCapAsync(page, pageSize);
                return Ok(cryptocurrencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching cryptocurrencies.");
            }
        }

        [HttpGet("arbitrage-opportunities/{baseCoinId}")]
        public async Task<IActionResult> GetArbitrageOpportunities(string baseCoinId)
        {
            try
            {
                var opportunities = await _arbitrageService.FindArbitrageOpportunitiesAsync(baseCoinId);
                return Ok(opportunities);
            }
            catch (ArgumentException ex)
            {
                // Handle specific errors, such as invalid arguments
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching arbitrage opportunities.");
            }
        }

        [HttpGet("chart/{coinId}")]
        public async Task<IActionResult> GetChartData(string coinId, [FromQuery] string interval)
        {
            try
            {
                // Fetch chart data
                var chartData = await _coinGeckoService.GetChartDataAsync(coinId, interval);
                if (chartData == null)
                {
                    return BadRequest("Invalid interval specified. Please use 'day', 'week', 'month', or 'year'.");
                }

                // Fetch coin details
                var coinDetail = await _coinGeckoService.GetCoinDetailAsync(coinId);
                if (coinDetail == null)
                {
                    return NotFound("Coin details not found.");
                }

                // Combine chart data and coin details into a single response
                var response = new CoinChartResponse
                {
                    Detail = coinDetail,
                    ChartData = chartData
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                // Handle invalid arguments specifically
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching chart data.");
            }
        }
    }
}
