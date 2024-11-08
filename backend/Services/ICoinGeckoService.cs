// File: CryptoArbitrageAPI/Services/ICoinGeckoService.cs

using CryptoArbitrageAPI.Models;

namespace CryptoArbitrageAPI.Services
{
    public interface ICoinGeckoService
    {
        Task<List<Cryptocurrency>> GetCryptocurrenciesByMarketCapAsync(int? page, int? pageSize);
        Task<Dictionary<string, decimal>> GetUsdPricesAsync(List<string> coinIds);
        Task<List<ExchangePair>> GetExchangePairsAsync();
        Task<List<PriceDataPoint>> GetChartDataAsync(string coinId, string interval);
        Task<Cryptocurrency> GetCoinDetailAsync(string coinId);
    }
}
