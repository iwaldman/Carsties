import CountdownTimer from '@/app/auctions/CountdownTimer'
import CarImage from '@/app/auctions/CarImage'
import { Auction } from '@/types'

type Props = {
  auction: Auction
}

const AuctionCard = (props: Props) => {
  const { auction } = props

  return (
    <a href='#' className='group'>
      <div className='relative w-full bg-gray-200 aspect-[16/10] rounded-lg overflow-hidden'>
        <CarImage imageUrl={auction.imageUrl} />
        <div className='absolute bottom-2 left-2'>
          <CountdownTimer auctionEnd={auction.auctionEnd} />
        </div>
      </div>
      <div className='flex justify-between items-center, mt-4'>
        <h3 className='text-gray-700'>
          {auction.make} {auction.model}
        </h3>
        <p className='font-semibold text-sm'>{auction.year}</p>
      </div>
    </a>
  )
}

export default AuctionCard
