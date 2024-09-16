'use server'

import { Auction, PagedResults } from '@/types'

async function getData(query: string): Promise<PagedResults<Auction>> {
  console.log('getData', query)
  const response = await fetch(`http://localhost:6001/search${query}`)

  if (!response.ok) {
    throw new Error('Failed to fetch data')
  }

  const data = await response.json()
  return data
}

export default getData
