using System.Text.Json.Serialization;

namespace CryptoArbitrageAPI.Models
{
    public class ExchangePair
    {
        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("market")]
        public Market Market { get; set; }

        [JsonPropertyName("last")]
        public decimal Last { get; set; }

        // Additional properties as needed
    }

    public class Market
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        // Additional properties as needed
    }

    public class ExchangeTickerResponse
    {
        [JsonPropertyName("tickers")]
        public List<ExchangePair> Tickers { get; set; }
    }

}
