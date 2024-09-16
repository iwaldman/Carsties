'use client'

import { Auction, PagedResults } from '@/types'
import AuctionCard from '@/components/AuctionCard'
import AppPagination from '@/components/AppPagination'
import { useState, useEffect } from 'react'
import getData from '@/app/actions/auctionActions'
import Filters from '@/components/Filters'
import { useShallow } from 'zustand/shallow'
import useParamsStore from '@/hooks/useParamsStore'
import qs from 'query-string'

const Listings = () => {
  const [data, setData] = useState<PagedResults<Auction>>()

  const params = useParamsStore(
    useShallow((state) => ({
      pageNumber: state.pageNumber,
      pageSize: state.pageSize,
      pageCount: state.pageCount,
      searchTerm: state.searchTerm,
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
  }, [url])

  if (!data) {
    return <div>Loading...</div>
  }

  return (
    <>
      <Filters />
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
  )
}

export default Listings
