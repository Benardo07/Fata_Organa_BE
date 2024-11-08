// File: CryptoArbitrageAPI/Models/ArbitrageOpportunity.cs

namespace CryptoArbitrageAPI.Models
{
    public class ArbitrageOpportunity
    {
        public List<string> Path { get; set; }
        public decimal ProfitPercentage { get; set; }
    }
}
