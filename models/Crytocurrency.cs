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
}
}