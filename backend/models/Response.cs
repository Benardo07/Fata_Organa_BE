
namespace CryptoArbitrageAPI.Models
{
    public class CoinChartResponse
    {
        public Cryptocurrency Detail { get; set; }
        public List<PriceDataPoint> ChartData { get; set; }
    }
}