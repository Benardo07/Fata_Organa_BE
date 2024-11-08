using CryptoArbitrageAPI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrageAPI.Services
{
    public class ArbitrageService : IArbitrageService
    {
        private readonly ICoinGeckoService _coinGeckoService;
        private readonly ILogger<ArbitrageService> _logger;

        // List of stablecoins to consider
        private readonly List<string> _stableCoins = new List<string> { "usdt", "usdc", "busd", "dai" };

        public ArbitrageService(ICoinGeckoService coinGeckoService, ILogger<ArbitrageService> logger)
        {
            _coinGeckoService = coinGeckoService;
            _logger = logger;
        }

        public async Task<List<ArbitrageOpportunity>> FindArbitrageOpportunitiesAsync(string baseCoinId, int maxPathLength = 4)
        {
            baseCoinId = baseCoinId.ToLower();

            // Step 1: Fetch top cryptocurrencies
            var coins = await _coinGeckoService.GetCryptocurrenciesByMarketCapAsync(null,null);
            var coinIds = coins.Select(c => c.Id.ToLower()).Take(100).ToList();

            // Ensure base coin is included
            if (!coinIds.Contains(baseCoinId))
            {
                coinIds.Add(baseCoinId);
            }

            // Step 2: Fetch USD prices
            var usdPrices = await _coinGeckoService.GetUsdPricesAsync(coinIds);

            // Step 3: Fetch exchange pairs (actual exchange rates between stablecoins and cryptocurrencies)
            var exchangePairs = await _coinGeckoService.GetExchangePairsAsync();

            // Filter exchange pairs to include only those involving stablecoins
            var filteredPairs = exchangePairs.Where(pair =>
                (_stableCoins.Contains(pair.Base.ToLower()) && coinIds.Contains(pair.Target.ToLower())) ||
                (coinIds.Contains(pair.Base.ToLower()) && _stableCoins.Contains(pair.Target.ToLower()))
            ).ToList();

            // Step 4: Build graph
            var graph = BuildGraph(coinIds, usdPrices, filteredPairs);

            // Step 5: Detect arbitrage opportunities
            var opportunities = DetectArbitrage(graph, baseCoinId, maxPathLength);

            // Step 6: Sort by ProfitPercentage and take the top 3 results
            var topOpportunities = opportunities
                .OrderByDescending(o => o.ProfitPercentage)
                .Take(3)
                .ToList();

            return topOpportunities;
        }

        private Dictionary<string, List<Edge>> BuildGraph(List<string> coinIds, Dictionary<string, decimal> usdPrices, List<ExchangePair> exchangePairs)
        {
            var graph = new Dictionary<string, List<Edge>>();

            // Initialize graph nodes
            foreach (var coin in coinIds)
            {
                graph[coin] = new List<Edge>();
            }

            // Add edges for stablecoin to coin exchanges using actual exchange rates
            foreach (var pair in exchangePairs)
            {
                var baseCoin = pair.Base.ToLower();
                var targetCoin = pair.Target.ToLower();
                var rate = pair.Last;

                // Ensure the rate is positive
                if (rate <= 0) continue;

                // Add edge from base to target
                if (graph.ContainsKey(baseCoin))
                {
                    graph[baseCoin].Add(new Edge
                    {
                        To = targetCoin,
                        Rate = rate
                    });
                }

                // Add reverse edge
                if (graph.ContainsKey(targetCoin))
                {
                    graph[targetCoin].Add(new Edge
                    {
                        To = baseCoin,
                        Rate = 1 / rate
                    });
                }
            }

            // Add edges between coins using manually calculated exchange rates
            foreach (var fromCoin in coinIds)
            {
                foreach (var toCoin in coinIds)
                {
                    if (fromCoin == toCoin) continue;

                    // Skip if there is already an edge between these coins
                    if (graph[fromCoin].Any(e => e.To == toCoin)) continue;

                    if (usdPrices.TryGetValue(fromCoin, out decimal fromPrice) && usdPrices.TryGetValue(toCoin, out decimal toPrice))
                    {
                        // Calculate exchange rate
                        decimal rate = fromPrice / toPrice;

                        // Add edge from fromCoin to toCoin
                        graph[fromCoin].Add(new Edge
                        {
                            To = toCoin,
                            Rate = rate
                        });

                        // Add reverse edge
                        graph[toCoin].Add(new Edge
                        {
                            To = fromCoin,
                            Rate = 1 / rate
                        });
                    }
                }
            }

            return graph;
        }

        private List<ArbitrageOpportunity> DetectArbitrage(Dictionary<string, List<Edge>> graph, string startCoin, int maxPathLength)
        {
            var opportunities = new List<ArbitrageOpportunity>();

            var path = new List<string> { startCoin };
            var visited = new HashSet<string> { startCoin };
            
            // Use DFS
            DFS(graph, startCoin, startCoin, 1.0m, path, visited, opportunities, maxPathLength);

            return opportunities;
        }

        private void DFS(Dictionary<string, List<Edge>> graph, string currentCoin, string startCoin, decimal accumulatedRate, List<string> path, HashSet<string> visited, List<ArbitrageOpportunity> opportunities, int maxPathLength)
        {
            if (path.Count > maxPathLength)
                return;

            if (path.Count >= 3 && currentCoin == startCoin && accumulatedRate > 1.0m)
            {
                // calculate profit 
                var profitPercentage = (accumulatedRate - 1.0m) * 100;
                opportunities.Add(new ArbitrageOpportunity
                {
                    Path = new List<string>(path),
                    ProfitPercentage = profitPercentage
                });
                return;
            }

            if (!graph.ContainsKey(currentCoin))
                return;

            foreach (var edge in graph[currentCoin])
            {
                if (!visited.Contains(edge.To) || (edge.To == startCoin && path.Count >= 3))
                {
                    path.Add(edge.To);
                    var newAccumulatedRate = accumulatedRate * edge.Rate;

                    if (edge.To == startCoin && path.Count >= 3 && newAccumulatedRate > 1.0m)
                    {
                        // Found a profitable cycle
                        var profitPercentage = (newAccumulatedRate - 1.0m) * 100;
                        opportunities.Add(new ArbitrageOpportunity
                        {
                            Path = new List<string>(path),
                            ProfitPercentage = profitPercentage
                        });
                    }
                    else if (!visited.Contains(edge.To))
                    {
                        visited.Add(edge.To);
                        DFS(graph, edge.To, startCoin, newAccumulatedRate, path, visited, opportunities, maxPathLength);
                        visited.Remove(edge.To);
                    }

                    // Backtrack
                    path.RemoveAt(path.Count - 1);
                }
            }
        }
    }

    public class Edge
    {
        public string To { get; set; }
        public decimal Rate { get; set; }
    }
}
