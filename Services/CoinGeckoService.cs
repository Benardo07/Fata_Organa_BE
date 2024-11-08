// File: CryptoArbitrageAPI/Services/CoinGeckoService.cs

using CryptoArbitrageAPI.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging; // Ensure you have this namespace for logging
namespace CryptoArbitrageAPI.Services
{
    public class CoinGeckoService : ICoinGeckoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CoinGeckoService> _logger; // Add ILogger

        public CoinGeckoService(HttpClient httpClient,ILogger<CoinGeckoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyCryptoApp/1.0");
            }

        }

        public async Task<List<Cryptocurrency>> GetCryptocurrenciesByMarketCapAsync()
        {
            var response = await _httpClient.GetAsync("coins/markets?vs_currency=usd");
            _logger.LogInformation("Response: {Content}",response);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response Content: {Content}", content);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var cryptocurrencies = JsonSerializer.Deserialize<List<Cryptocurrency>>(content, options);
            if (cryptocurrencies == null)
            {
                _logger.LogError("Failed to deserialize cryptocurrencies.");
                return new List<Cryptocurrency>();
            }
            return cryptocurrencies;
        }
        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(List<string> coinIds)
        {
            var ids = string.Join(",", coinIds);
            var response = await _httpClient.GetAsync($"simple/price?ids={ids}&vs_currencies=usd&x-cg-demo-api-key=CG-cCGpeAG4hmPBzuRBkAzpW2dt");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response Content: {Content}", content);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rates = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(content, options);

            // Flatten the rates dictionary
            var exchangeRates = new Dictionary<string, decimal>();
            foreach (var coin in rates)
            {
                if (coin.Value != null && coin.Value.TryGetValue("usd", out decimal usdValue))
                {
                    exchangeRates[coin.Key] = usdValue;
                }
                else
                {
                    _logger.LogWarning("USD value not found for coin: {Coin}", coin.Key);
                    continue;
                }
            }

            return exchangeRates;
        }

        public async Task<List<ExchangePair>> GetExchangePairsAsync()
        {
            var requestUri = "exchanges/binance/tickers";

            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API Error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                throw new HttpRequestException($"API Error: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();


            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<ExchangeTickerResponse>(content, options);
            if (data == null || data.Tickers == null)
            {
                _logger.LogError("Failed to deserialize exchange pairs.");
                return new List<ExchangePair>();
            }

            var tickersJson = JsonSerializer.Serialize(data.Tickers, options);
            _logger.LogInformation("Tickers Data: {TickersJson}", tickersJson);
            return data.Tickers;
        }

      


    }
}
