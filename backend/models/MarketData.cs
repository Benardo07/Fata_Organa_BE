// File: CryptoArbitrageAPI/Models/MarketData.cs

using System.Text.Json.Serialization;

namespace CryptoArbitrageAPI.Models
{
    public class ChartDataResponse
    {
        [JsonPropertyName("prices")]
        public List<List<double>> Prices { get; set; }
    }    
    public class PriceDataPoint
    {
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
    }

}
