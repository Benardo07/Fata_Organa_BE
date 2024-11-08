// File: CryptoArbitrageAPI/Controllers/CryptoController.cs

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
        public async Task<IActionResult> GetCryptocurrencies()
        {
            var cryptocurrencies = await _coinGeckoService.GetCryptocurrenciesByMarketCapAsync();
            
            return Ok(cryptocurrencies);
        }

        [HttpGet("arbitrage-opportunities/{baseCoinId}")]
        public async Task<IActionResult> GetArbitrageOpportunities(string baseCoinId)
        {
            var opportunities = await _arbitrageService.FindArbitrageOpportunitiesAsync(baseCoinId);
            return Ok(opportunities);
        }
    }
}
