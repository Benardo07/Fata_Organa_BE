'use client'

import { useState, useEffect } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination"
import { ArrowUpDown, ChevronUp, ChevronDown } from "lucide-react"
import Image from 'next/image'
import { useRouter } from 'next/navigation'

// Import skeleton styling
import {Skeleton} from '@/components/ui/skeleton'
import { formatNumberWithUnits, formatPercentage } from '@/utils/util'

export interface CryptoData {
  id: string;
  symbol: string;
  name: string;
  image: string;
  current_price: number;
  price_change_percentage_24h: number;
  total_volume: number;
  market_cap: number;
  high_24h: number;
  low_24h: number;
  circulating_supply: number;
  total_supply: number | null;
  max_supply: number | null;
}

interface ArbitrageData {
  path: string[];
  profitPercentage: number;
}




export default function Dashboard() {
  const [cryptoData, setCryptoData] = useState<CryptoData[]>([])
  const [arbitrageData, setArbitrageData] = useState<ArbitrageData[]>([])
  const [currentPage, setCurrentPage] = useState(1)
  const itemsPerPage = 20
  const totalPages = 12
  const [visiblePages, setVisiblePages] = useState(5)
  const [loadingCrypto, setLoadingCrypto] = useState(true)
  const [loadingArbitrage, setLoadingArbitrage] = useState(true)
  const router = useRouter()

  useEffect(() => {
    const fetchCryptocurrencies = async () => {
      setLoadingCrypto(true)
      try {
        const response = await fetch(`http://localhost:5232/api/Crypto/cryptocurrencies?page=${currentPage}&pageSize=${itemsPerPage}`)
        const data: CryptoData[] = await response.json()
        setCryptoData(data)
      } catch (error) {
        console.error("Failed to fetch cryptocurrencies", error)
      }
      setLoadingCrypto(false)
    }

    fetchCryptocurrencies()
  }, [currentPage, itemsPerPage])

  useEffect(() => {
    const fetchArbitrageOpportunities = async () => {
      setLoadingArbitrage(true)
      try {
        const response = await fetch(`http://localhost:5232/api/Crypto/arbitrage-opportunities/usdt`)
        const data: ArbitrageData[] = await response.json()
        const formattedData = data.map((item) => ({
          ...item,
          path: item.path.map((symbol) => symbol.toUpperCase())
        }))
        setArbitrageData(formattedData)
      } catch (error) {
        console.error("Failed to fetch arbitrage opportunities", error)
      }
      setLoadingArbitrage(false)
    }

    fetchArbitrageOpportunities()
    const interval = setInterval(fetchArbitrageOpportunities, 10000)

    return () => clearInterval(interval)
  }, [])

  const renderPaginationItems = () => {
    const halfVisible = Math.floor(visiblePages / 2)
    let startPage = Math.max(1, currentPage - halfVisible)
    let endPage = Math.min(totalPages, currentPage + halfVisible)

    if (endPage - startPage + 1 < visiblePages) {
      if (currentPage <= halfVisible) {
        endPage = Math.min(totalPages, visiblePages)
      } else if (currentPage + halfVisible >= totalPages) {
        startPage = Math.max(1, totalPages - visiblePages + 1)
      }
    }

    const paginationItems = []
    for (let i = startPage; i <= endPage; i++) {
      paginationItems.push(
        <PaginationItem key={i}>
          <PaginationLink
            href="#"
            onClick={() => setCurrentPage(i)}
            isActive={currentPage === i}
            className="text-gray-400 hover:text-white data-[active]:bg-[#2a2a2a]"
          >
            {i}
          </PaginationLink>
        </PaginationItem>
      )
    }

    return paginationItems
  }

  return (
    <div className="min-h-screen bg-[#121212] text-white">
      <div className="container mx-auto px-4 py-20">
        {/* Arbitrage Opportunities with Skeleton Loader */}
        <Card className="mb-6 bg-[#1a1a1a] border-[#2a2a2a]">
          <CardHeader>
            <CardTitle className="text-white">Top 3 Arbitrage Suggestions</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid gap-4 md:grid-cols-3">
              {loadingArbitrage ? (
                Array.from({ length: 3 }).map((_, i) => (
                  <Skeleton key={i} className="bg-[#2a2a2a] p-4 rounded-lg h-24" />
                ))
              ) : (
                arbitrageData.map((opportunity, index) => (
                  <div key={index} className="bg-[#242424] p-4 rounded-lg border border-[#2a2a2a]">
                    <div className="text-lg font-medium text-white mb-2">{opportunity.path.join(' → ')}</div>
                    <div className="text-green-400 text-lg font-bold">+{formatPercentage(opportunity.profitPercentage)}%</div>
                  </div>
                ))
              )}
            </div>
          </CardContent>
        </Card>

        {/* Main Tabs with Cryptocurrency List and Skeleton Loader */}
        <Tabs defaultValue="overview" className="space-y-4">
          <TabsList className="bg-[#1a1a1a] border-b border-[#2a2a2a]">
            <TabsTrigger value="overview" className="text-gray-400 data-[state=active]:text-white">
              Market Overview
            </TabsTrigger>
          </TabsList>

          <TabsContent value="overview">
            <Card className="bg-[#1a1a1a] border-[#2a2a2a]">
              <CardContent className="p-0">
                <Table>
                  <TableHeader>
                    <TableRow className="border-[#2a2a2a] hover:bg-[#242424]">
                      <TableHead className="text-gray-400">Nama</TableHead>
                      <TableHead className="text-gray-400">Harga</TableHead>
                      <TableHead className="text-gray-400">Perubahan 24h</TableHead>
                      <TableHead className="text-gray-400">Volume 24jam</TableHead>
                      <TableHead className="text-gray-400">Kap Pasar</TableHead>
                      <TableHead className="text-gray-400">Tindakan</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {loadingCrypto
                      ? Array.from({ length: itemsPerPage }).map((_, i) => (
                          <TableRow key={i} className="border-[#2a2a2a] hover:bg-[#242424]">
                            <TableCell><Skeleton className="h-6 w-24 bg-[#2a2a2a]" /></TableCell>
                            <TableCell><Skeleton className="h-6 w-16 bg-[#2a2a2a]" /></TableCell>
                            <TableCell><Skeleton className="h-6 w-12 bg-[#2a2a2a]" /></TableCell>
                            <TableCell><Skeleton className="h-6 w-20 bg-[#2a2a2a]" /></TableCell>
                            <TableCell><Skeleton className="h-6 w-20 bg-[#2a2a2a]" /></TableCell>
                            <TableCell><Skeleton className="h-6 w-12 bg-[#2a2a2a]" /></TableCell>
                          </TableRow>
                        ))
                      : cryptoData.map((crypto) => (
                          <TableRow
                            key={crypto.id}
                            className="border-[#2a2a2a] hover:bg-[#242424] cursor-pointer"
                            onClick={() => router.push(`/cryptodetail/${crypto.id}`)}
                          >
                            <TableCell className="font-medium text-white">
                              <div className="flex items-center gap-2">
                                <Image src={crypto.image} alt={crypto.name} width={24} height={24} className="w-6 h-6 rounded-full" />
                                <span>{crypto.name}</span>
                                <span className="text-gray-400">{crypto.symbol}</span>
                              </div>
                            </TableCell>
                            <TableCell className="text-white">
                              ${crypto.current_price.toLocaleString()}
                            </TableCell>
                            <TableCell>
                              <div className={`flex items-center ${crypto.price_change_percentage_24h >= 0 ? 'text-green-400' : 'text-red-400'}`}>
                                {crypto.price_change_percentage_24h >= 0 ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
                                {formatPercentage(Math.abs(crypto.price_change_percentage_24h))}%
                              </div>
                            </TableCell>
                            <TableCell className="text-white">
                              ${formatNumberWithUnits(crypto.total_volume)}
                            </TableCell>
                            <TableCell className="text-white">
                              ${formatNumberWithUnits(crypto.market_cap)}
                            </TableCell>
                            <TableCell>
                              <button
                                className="p-2 hover:bg-[#2a2a2a] rounded-lg"
                                title="View Coin Detail"
                                onClick={(e) => {
                                  e.stopPropagation()
                                  router.push(`/cryptodetail/${crypto.id}`)
                                }}
                              >
                                <ArrowUpDown className="h-4 w-4 text-gray-400" />
                              </button>
                            </TableCell>
                          </TableRow>
                        ))}
                  </TableBody>
                </Table>

                <div className="py-4 px-6 w-full overflow-auto">
                  <Pagination className="text-sm">
                    <PaginationContent>
                      <PaginationItem>
                        <PaginationPrevious
                          href="#"
                          onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
                          className="text-gray-400 hover:text-white"
                        />
                      </PaginationItem>
                      {currentPage > 1 && <PaginationEllipsis className="text-gray-400" />}
                      {renderPaginationItems()}
                      {currentPage < totalPages && <PaginationEllipsis className="text-gray-400" />}
                      <PaginationItem>
                        <PaginationNext
                          href="#"
                          onClick={() => setCurrentPage(Math.min(totalPages, currentPage + 1))}
                          className="text-gray-400 hover:text-white"
                        />
                      </PaginationItem>
                    </PaginationContent>
                  </Pagination>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  )
}
