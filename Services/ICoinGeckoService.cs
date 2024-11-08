// File: CryptoArbitrageAPI/Services/ICoinGeckoService.cs

using CryptoArbitrageAPI.Models;

namespace CryptoArbitrageAPI.Services
{
    public interface ICoinGeckoService
    {
        Task<List<Cryptocurrency>> GetCryptocurrenciesByMarketCapAsync();
        Task<Dictionary<string, decimal>> GetExchangeRatesAsync(List<string> coinIds);
        Task<List<ExchangePair>> GetExchangePairsAsync();
    }
}
