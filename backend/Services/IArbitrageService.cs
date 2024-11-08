// File: CryptoArbitrageAPI/Services/IArbitrageService.cs

using CryptoArbitrageAPI.Models;

namespace CryptoArbitrageAPI.Services
{
    public interface IArbitrageService
    {
        Task<List<ArbitrageOpportunity>> FindArbitrageOpportunitiesAsync(string baseCoinId, int maxPathLength = 3);
    }
}
