'use client'

import { useState, useEffect } from 'react'
import { ChevronRight, Share2 } from 'lucide-react'
import Link from 'next/link'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableRow } from '@/components/ui/table'
import { LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts'
import { CryptoData, formatNumberWithUnits } from '@/app/page'
import Image from 'next/image'
import { Skeleton } from '@/components/ui/skeleton'  // Ensure Skeleton component is imported

interface DetailProps {
  params: {
    id: string;
  };
}

interface ChartData {
  timestamp: string;
  price: number;
}

interface ApiResponse {
  detail: CryptoData;
  chartData: ChartData[];
}

const intervalMapping: Record<string, string> = {
  '1D': 'day',
  '7D': 'week',
  '1M': 'month',
  '3M': '3month',
  '1Y': 'year',
};

export default function CryptoDetail({ params }: DetailProps) {
  const { id } = params;
  const [timeframe, setTimeframe] = useState('1D');
  const [chartData, setChartData] = useState<ChartData[]>([]);
  const [coinDetail, setCoinDetail] = useState<CryptoData | null>(null);
  const [loadingCoinDetail, setLoadingCoinDetail] = useState(true); // For initial load of coin details
  const [loadingChart, setLoadingChart] = useState(true); // For chart skeleton loading

  // Initial fetch to get both coin details and chart data
  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        setLoadingCoinDetail(true); // Start loading skeleton for coin detail
        const response = await fetch(`http://localhost:5232/api/Crypto/chart/${id}?interval=${intervalMapping[timeframe]}&includeDetails=true`);
        const data: ApiResponse = await response.json();
        
        setCoinDetail(data.detail);
        setChartData(data.chartData);
        setLoadingCoinDetail(false); // Stop loading skeleton for coin detail after initial load
        setLoadingChart(false); // Stop loading skeleton for chart
      } catch (error) {
        console.error('Failed to fetch initial data', error);
      }
    };

    fetchInitialData();
  }, [id]);

  // Fetch only chart data on interval change
  useEffect(() => {
    if (!id || loadingCoinDetail) return; // Skip if coin details are not yet loaded

    const fetchChartData = async () => {
      setLoadingChart(true); // Start loading skeleton for chart
      try {
        const response = await fetch(`http://localhost:5232/api/Crypto/chart/${id}?interval=${intervalMapping[timeframe]}&includeDetails=false`);
        const data = await response.json();
        
        setChartData(data.chartData);
        setLoadingChart(false); // Stop loading skeleton for chart
      } catch (error) {
        console.error('Failed to fetch chart data', error);
        setLoadingChart(false);
      }
    };

    fetchChartData();
  }, [id, timeframe, loadingCoinDetail]);

  const formatXAxis = (value: string | number) => {
    const date = new Date(value as number);
    switch (timeframe) {
      case '1D':
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
      case '7D':
        return date.toLocaleDateString([], { day: '2-digit', month: 'short' });
      case '1M':
      case '3M':
        return date.toLocaleDateString([], { day: '2-digit', month: 'short' });
      case '1Y':
        return date.toLocaleDateString([], { month: 'short' });
      default:
        return date.toLocaleDateString();
    }
  };

  return (
    <div className="min-h-screen bg-[#121212] text-white">
      <div className="container mx-auto px-4 py-20 max-w-7xl">
        {/* Breadcrumb */}
        <div className="flex items-center gap-2 text-sm text-gray-400 mb-6">
          <Link href="/" className="hover:text-white">Home</Link>
          <ChevronRight className="h-4 w-4" />
          <span className="text-white">{loadingCoinDetail ? <Skeleton className="w-32 h-6 bg-gray-700" /> : `${coinDetail?.name} Price`}</span>
        </div>

        <div className="grid gap-6 lg:grid-cols-[1fr_400px] md:grid-cols-[1fr_350px]">
          <div className="space-y-6">
            {/* Coin Header */}
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-4">
                {loadingCoinDetail ? (
                  <Skeleton className="w-12 h-12 rounded-full" />
                ) : (
                  <Image
                    src={coinDetail!.image}
                    alt={coinDetail!.name}
                    width={48}
                    height={48}
                    className="w-12 h-12 rounded-full"
                  />
                )}
                <div>
                  <h1 className="text-2xl font-bold flex items-center gap-2">
                    {loadingCoinDetail ? (
                      <Skeleton className="w-24 h-6 bg-gray-700" />
                    ) : (
                      <>
                        {coinDetail?.name} Price
                        <span className="text-gray-400 text-lg">({coinDetail?.symbol.toUpperCase()})</span>
                      </>
                    )}
                  </h1>
                  <div className="flex items-center gap-2">
                    <span className="text-2xl font-bold">
                      {loadingCoinDetail ? <Skeleton className="w-16 h-6 bg-gray-700" /> : `$${coinDetail?.current_price.toLocaleString()}`}
                    </span>
                    {coinDetail && (
                      <span className={`text-lg ${coinDetail.price_change_percentage_24h >= 0 ? 'text-green-400' : 'text-red-400'}`}>
                        {coinDetail.price_change_percentage_24h.toFixed(2)}%
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <Button variant="outline" size="icon">
                <Share2 className="h-4 w-4" />
              </Button>
            </div>

            {/* Chart Controls */}
            <div className="flex items-center gap-2">
              {['1D', '7D', '1M', '3M', '1Y'].map((period) => (
                <Button
                  key={period}
                  variant={timeframe === period ? 'default' : 'outline'}
                  onClick={() => setTimeframe(period)}
                  className={`min-w-[60px] transition-colors ${
                    timeframe === period 
                      ? 'bg-white bg-opacity-25 text-primary-foreground hover:bg-primary/90' 
                      : 'hover:bg-white hover:bg-opacity-15'
                  }`}
                >
                  {period}
                </Button>
              ))}
            </div>

            {/* Chart */}
            <Card className="bg-[#1a1a1a] border-[#2a2a2a]">
              <CardContent className="p-6">
                <div className="h-[400px]">
                  {loadingChart ? (
                    <Skeleton className="h-full w-full bg-gray-700 rounded-md" />
                  ) : (
                    <ResponsiveContainer width="100%" height="100%">
                      <LineChart data={chartData}>
                        <XAxis 
                          dataKey="timestamp" 
                          stroke="#666" 
                          tickFormatter={formatXAxis}
                          interval={timeframe === '1Y' ? 30 : 'preserveStartEnd'} 
                          minTickGap={30}
                        />
                        <YAxis 
                          stroke="#666"
                          domain={['auto', 'auto']}
                          tickFormatter={(value) => `$${value.toFixed(2)}`}
                        />
                        <Tooltip
                          contentStyle={{ backgroundColor: '#242424', border: 'none' }}
                          labelStyle={{ color: '#999' }}
                          itemStyle={{ color: '#fff' }}
                          formatter={(value) => {
                            const formattedValue = typeof value === 'number' ? `$${value.toFixed(2)}` : value;
                            return [formattedValue, 'Price'];
                          }}
                          labelFormatter={(label) => new Date(label as number).toLocaleDateString()} 
                        />
                        <Line 
                          type="monotone" 
                          dataKey="price" 
                          stroke="#ffd700" 
                          dot={false}
                          strokeWidth={2}
                        />
                      </LineChart>
                    </ResponsiveContainer>
                  )}
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Right Sidebar */}
          <div className="space-y-6">
            <Card className="bg-[#1a1a1a] border-[#2a2a2a]">
              <CardContent className="p-6">
                <h3 className="text-lg font-semibold mb-4">Market Stats</h3>
                <Table>
                  <TableBody>
                    {loadingCoinDetail ? (
                      Array.from({ length: 6 }).map((_, i) => (
                        <TableRow key={i}>
                          <TableCell><Skeleton className="w-full h-6 bg-gray-700" /></TableCell>
                          <TableCell><Skeleton className="w-full h-6 bg-gray-700" /></TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <>
                        <TableRow>
                          <TableCell className="text-gray-400">Market Cap</TableCell>
                          <TableCell className="text-right">${formatNumberWithUnits(coinDetail!.market_cap)}</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell className="text-gray-400">24h High</TableCell>
                          <TableCell className="text-right">${coinDetail!.high_24h.toLocaleString()}</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell className="text-gray-400">24h Low</TableCell>
                          <TableCell className="text-right">${coinDetail!.low_24h.toLocaleString()}</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell className="text-gray-400">Circulating Supply</TableCell>
                          <TableCell className="text-right">{coinDetail!.circulating_supply.toLocaleString()}</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell className="text-gray-400">Total Supply</TableCell>
                          <TableCell className="text-right">{coinDetail!.total_supply ? coinDetail!.total_supply.toLocaleString() : 'N/A'}</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell className="text-gray-400">Max Supply</TableCell>
                          <TableCell className="text-right">{coinDetail!.max_supply ? coinDetail!.max_supply.toLocaleString() : 'N/A'}</TableCell>
                        </TableRow>
                      </>
                    )}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
}
