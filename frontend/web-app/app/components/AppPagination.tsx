'use client'

import { Pagination } from 'flowbite-react'
import React from 'react'

type Props = {
  currentPage: number
  pageCount: number
  pageChanged: (page: number) => void
}

function AppPagination({ currentPage, pageCount, pageChanged }: Props) {
  return (
    <Pagination
      currentPage={currentPage}
      onPageChange={pageChanged}
      totalPages={pageCount}
      layout="pagination"
      showIcons={true}
      className="text-blue-500"
    />
  )
}

export default AppPagination
