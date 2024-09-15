'use client'

import { Auction, PagedResults } from '@/types'
import AuctionCard from '@/components/AuctionCard'
import AppPagination from '@/components/AppPagination'
import { useState, useEffect } from 'react'
import getData from '@/app/actions/auctionActions'

const Listings = () => {
  const [auctions, setAuctions] = useState<Auction[]>([])
  const [pageCount, setPageCount] = useState(0)
  const [pageNumber, setPageNumber] = useState(1)

  useEffect(() => {
    getData(pageNumber).then((data: PagedResults<Auction>) => {
      setAuctions(data.results)
      setPageCount(data.pageCount)
    })
  }, [pageNumber])

  if (auctions.length === 0) {
    return <div>Loading...</div>
  }

  return (
    <>
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
