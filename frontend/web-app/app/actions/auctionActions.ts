'use server'

import { Auction, PagedResult } from "@/types"

export async function getData(query: string): Promise<PagedResult<Auction>> {
  const url = `http://localhost:6001/search${query}`

  const res = await fetch(url)

  if (!res.ok) {
    throw new Error(`Could not fetch ${url}, received ${res.status}`)
  }

  const auctions = await res.json()
  return auctions
}