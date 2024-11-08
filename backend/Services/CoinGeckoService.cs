// File: CryptoArbitrageAPI/Services/CoinGeckoService.cs

using CryptoArbitrageAPI.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
namespace CryptoArbitrageAPI.Services
{
    public class CoinGeckoService : ICoinGeckoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CoinGeckoService> _logger; 

        public CoinGeckoService(HttpClient httpClient,ILogger<CoinGeckoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyCryptoApp/1.0");
            }

        }

        public async Task<List<Cryptocurrency>> GetCryptocurrenciesByMarketCapAsync(int? page = null, int? pageSize = null)
        {   
            var requestUri = "";
            if (page.HasValue && pageSize.HasValue)
            {
                requestUri = $"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={pageSize}&page={page}";
            }else {
                requestUri = "coins/markets?vs_currency=usd&order=market_cap_desc";
            }
            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

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
        public async Task<Dictionary<string, decimal>> GetUsdPricesAsync(List<string> coinIds)
        {
            var ids = string.Join(",", coinIds);
            var response = await _httpClient.GetAsync($"simple/price?ids={ids}&vs_currencies=usd");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rates = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(content, options);

            if (rates == null)
            {
                _logger.LogError("Failed to deserialize USD prices.");
                return new Dictionary<string, decimal>();
            }

            // Flatten the rates dictionary
            var usdPrices = new Dictionary<string, decimal>();
            foreach (var coin in rates)
            {
                if (coin.Value != null && coin.Value.TryGetValue("usd", out decimal usdValue))
                {
                    usdPrices[coin.Key] = usdValue;
                }
                else
                {
                    _logger.LogWarning("USD price not found for coin: {Coin}", coin.Key);
                }
            }

            return usdPrices;
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
            
            return data.Tickers;
        }

        public async Task<Cryptocurrency> GetCoinDetailAsync(string coinId)
        {
            var requestUri = $"coins/markets?vs_currency=usd&ids={coinId}";
            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var coinDetails = JsonSerializer.Deserialize<List<Cryptocurrency>>(content, options);
            return coinDetails?.FirstOrDefault();
        }

        public async Task<List<PriceDataPoint>> GetChartDataAsync(string coinId, string interval)
        {
            // Determine the number of days based on the interval
            int days;
            switch (interval.ToLower())
            {
                case "day":
                    days = 1;
                    break;
                case "week":
                    days = 7;
                    break;
                case "month":
                    days = 30;
                    break;
                case "3month":
                    days = 90;
                    break;
                case "year":
                    days = 365;
                    break;
                default:
                    _logger.LogError("Invalid interval: {Interval}", interval);
                    return null;
            }

            // Build request URI for the CoinGecko API
            var requestUri = $"coins/{coinId}/market_chart?vs_currency=usd&days={days}";

            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserialize the response to ChartDataResponse
            var chartDataResponse = JsonSerializer.Deserialize<ChartDataResponse>(content, options);
            if (chartDataResponse == null || chartDataResponse.Prices == null)
            {
                _logger.LogError("Failed to deserialize chart data.");
                return null;
            }

            // Map the deserialized data to List<PriceDataPoint>
            var priceDataPoints = chartDataResponse.Prices.Select(p => new PriceDataPoint
            {
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)p[0]).UtcDateTime,
                Price = (decimal)p[1]
            }).ToList();

            return priceDataPoints;
        }



      


    }
}
