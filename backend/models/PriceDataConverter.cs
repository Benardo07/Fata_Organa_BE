using System.Text.Json;
using System.Text.Json.Serialization;


namespace CryptoArbitrageAPI.Models
{
    public class PriceDataPointConverter : JsonConverter<PriceDataPoint>
    {
        public override PriceDataPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            reader.Read();
            long timestamp = reader.GetInt64();

            reader.Read();
            decimal price = reader.GetDecimal();

            reader.Read(); // Move past the end of the array

            return new PriceDataPoint(timestamp, price);
        }

        public override void Write(Utf8JsonWriter writer, PriceDataPoint value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Timestamp);
            writer.WriteNumberValue(value.Price);
            writer.WriteEndArray();
        }
    }
}