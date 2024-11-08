using System.Text.Json.Serialization;

namespace CryptoArbitrageAPI.Models
{
    public class Cryptocurrency
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("high_24h")]
        public decimal High24h {get; set;}

        [JsonPropertyName("low_24h")]
        public decimal Low24h {get; set;}

        [JsonPropertyName("circulating_supply")]
        public decimal? CirculatingSupply {get; set;}

        [JsonPropertyName("total_supply")]
        public decimal? TotalSupply {get; set;}

        [JsonPropertyName("max_supply")]
        public decimal? MaxSupply {get; set;}


        [JsonPropertyName("current_price")]
        public decimal CurrentPrice {get; set; }

        [JsonPropertyName("price_change_percentage_24h")]
        public decimal Change24h { get; set; }  // Changed to decimal

        [JsonPropertyName("total_volume")]
        public decimal Volume24h { get; set; }  // Changed to decimal

        [JsonPropertyName("market_cap")]
        public decimal MarketCap { get; set; }  // Changed to decimal
    }
}