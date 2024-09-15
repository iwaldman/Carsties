'use server'

import { Auction, PagedResults } from '@/types'

async function getData(pageNumber: number = 1): Promise<PagedResults<Auction>> {
  const response = await fetch(`http://localhost:6001/search?pageSize=4&pageNumber=${pageNumber}`)

  if (!response.ok) {
    throw new Error('Failed to fetch data')
  }

  const data = await response.json()
  return data
}

export default getData
