import AuctionCard from '@/components/AuctionCard'
import { Auction, PagedResults } from '@/types'

async function getData(): Promise<PagedResults<Auction>> {
  const response = await fetch('http://localhost:6001/search')

  if (!response.ok) {
    throw new Error('Failed to fetch data')
  }

  const data = await response.json()
  return data
}

const Listings = async () => {
  const data = await getData()

  return (
    <div className='grid grid-cols-4 gap-6'>
      {data?.results?.map((auction) => (
        <AuctionCard auction={auction} key={auction.id} />
      ))}
    </div>
  )
}

export default Listings
