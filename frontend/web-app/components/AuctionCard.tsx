import Image from 'next/image'

type Props = {
  auction: any
}

const AuctionCard = (props: Props) => {
  const { auction } = props

  return (
    <a href='#'>
      <div className='relative w-full bg-gray-200 aspect-video rounded-lg overflow-hidden'>
        <Image
          src={auction.imageUrl}
          alt={`Image of ${auction.make} ${auction.model} in ${auction.color}`}
          fill
          className='object-cover'
          sizes='(max-width: 768px): 100vw, (max-width: 1200px): 50vw, 25vw'
          priority
        />
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
