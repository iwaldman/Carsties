'use server'

import { Auction, PagedResults } from '@/types'

export async function getData(query: string): Promise<PagedResults<Auction>> {
  console.log('getData', query)
  const response = await fetch(`http://localhost:6001/search${query}`)

  if (!response.ok) {
    throw new Error('Failed to fetch data')
  }

  const data = await response.json()
  return data
}

export async function updateAuctionTest() {
  const data = {
    mileage: Math.floor(Math.random() * 10000) + 1,
  }

  const res = await fetch(`http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(data),
  })

  if (!res.ok) {
    return { status: res.status, message: res.statusText }
  }

  return res.json()
}
