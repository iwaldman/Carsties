'use client'

import { Auction, PagedResults } from '@/types'
import AuctionCard from '@/app/auctions/AuctionCard'
import AppPagination from '@/components/AppPagination'
import { useState, useEffect } from 'react'
import getData from '@/app/actions/auctionActions'
import Filters from '@/app/auctions/Filters'
import { useShallow } from 'zustand/shallow'
import { useParamsStore } from '@/hooks/useParamsStore'
import qs from 'query-string'
import EmptyFilter from '@/components/EmptyFilter'

const Listings = () => {
  const [data, setData] = useState<PagedResults<Auction>>()

  const params = useParamsStore(
    useShallow((state) => ({
      pageNumber: state.pageNumber,
      pageSize: state.pageSize,
      searchTerm: state.searchTerm,
      orderBy: state.orderBy,
      filterBy: state.filterBy,
      seller: state.seller,
      winner: state.winner,
    }))
  )

  const setParams = useParamsStore((state) => state.setParams)
  const url = qs.stringifyUrl({ url: '', query: params })

  function setPageNumber(pageNumber: number) {
    setParams({ pageNumber })
  }

  useEffect(() => {
    getData(url).then((data: PagedResults<Auction>) => {
      setData(data)
    })
  }, [url, setData])

  if (!data) {
    return <h3>Loading...</h3>
  }

  return (
    <>
      <Filters />
      {data.totalCount === 0 ? (
        <EmptyFilter showReset />
      ) : (
        <>
          <div className='grid grid-cols-4 gap-6'>
            {data.results.map((auction) => (
              <AuctionCard key={auction.id} auction={auction} />
            ))}
          </div>
          <div className='flex justify-center mt-4'>
            <AppPagination
              pageChanged={setPageNumber}
              currentPage={params.pageNumber}
              pageCount={data.pageCount}
            />
          </div>
        </>
      )}
    </>
  )
}

export default Listings
