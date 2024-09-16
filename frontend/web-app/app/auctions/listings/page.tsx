'use client'

import { Auction, PagedResults } from '@/types'
import AuctionCard from '@/components/AuctionCard'
import AppPagination from '@/components/AppPagination'
import { useState, useEffect } from 'react'
import getData from '@/app/actions/auctionActions'
import Filters from '@/components/Filters'

const Listings = () => {
  const [auctions, setAuctions] = useState<Auction[]>([])
  const [pageCount, setPageCount] = useState(0)
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(4)

  useEffect(() => {
    getData(pageNumber, pageSize).then((data: PagedResults<Auction>) => {
      setAuctions(data.results)
      setPageCount(data.pageCount)
    })
  }, [pageNumber, pageSize])

  if (auctions.length === 0) {
    return <div>Loading...</div>
  }

  return (
    <>
      <Filters pageSize={pageSize} setPageSize={setPageSize} />
      <div className='grid grid-cols-4 gap-6'>
        {auctions.map((auction) => (
          <AuctionCard auction={auction} key={auction.id} />
        ))}
      </div>
      <div className='flex justify-center mt-4'>
        <AppPagination currentPage={1} pageCount={pageCount} pageChanged={setPageNumber} />
      </div>
    </>
  )
}

export default Listings
