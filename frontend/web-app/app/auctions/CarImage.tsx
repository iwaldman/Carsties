'use client'

import Image from 'next/image'
import { useState } from 'react'

type CarImageProps = {
  imageUrl: string
}

const CarImage = ({ imageUrl }: CarImageProps) => {
  const [isLoading, setIsLoading] = useState(true)

  return (
    <Image
      src={imageUrl}
      alt='Image of a car'
      fill
      className={`object-cover group-hover:opacity-75 duration-700 ease-in-out ${
        isLoading ? 'grayscale blur-2xl scale-10' : 'grayscale-0 blur-0 scale-100'
      }`}
      sizes='(max-width: 768px): 100vw, (max-width: 1200px): 50vw, 25vw'
      priority
      onLoad={() => setIsLoading(false)}
    />
  )
}

export default CarImage
