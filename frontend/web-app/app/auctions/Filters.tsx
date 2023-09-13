import { Button } from 'flowbite-react'
import React from 'react'
import { useParamsStore } from '../hooks/useParamsStore'
import { AiOutlineClockCircle, AiOutlineSortAscending } from 'react-icons/ai'
import { BsFillStopCircleFill, BsFillStopwatchFill } from 'react-icons/bs'
import { GiFinishLine, GiFlame } from 'react-icons/gi'

type Props = {}

const pageSizeButtons = [4, 8, 12]

const orderButtons = [
  { label: 'Alphabetical', icon: AiOutlineSortAscending, value: 'make' },
  { label: 'End date', icon: AiOutlineClockCircle, value: 'endingSoon' },
  { label: 'Recently added', icon: BsFillStopCircleFill, value: 'new' },
]

const filterButtons = [
  { label: 'Live Auctions', icon: GiFlame, value: 'live' },
  { label: 'Ending < 6 hours', icon: GiFinishLine, value: 'endingSoon' },
  { label: 'Completed', icon: BsFillStopwatchFill, value: 'finished' },
]

function Filters() {
  const pageSize = useParamsStore((state) => state.pageSize)
  const setParams = useParamsStore((state) => state.setParams)
  const orderBy = useParamsStore((state) => state.orderBy)
  const filterBy = useParamsStore((state) => state.filterBy)

  return (
    <div className="flex justify-between items-center mb-4">
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Filter by</span>
        <Button.Group>
          {filterButtons.map(({ label, icon: Icon, value }, i) => (
            <Button
              key={i}
              color={`${filterBy === value ? 'red' : 'gray'}`}
              onClick={() => setParams({ filterBy: value })}
            >
              <Icon className="mr-3 h-4 w-4" />
              {label}
            </Button>
          ))}
        </Button.Group>
      </div>
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Order by</span>
        <Button.Group>
          {orderButtons.map(({ label, icon: Icon, value }, i) => (
            <Button
              key={i}
              color={`${orderBy === value ? 'red' : 'gray'}`}
              onClick={() => setParams({ orderBy: value })}
            >
              <Icon className="mr-3 h-4 w-4" />
              {label}
            </Button>
          ))}
        </Button.Group>
      </div>
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Page size</span>
        <Button.Group>
          {pageSizeButtons.map((value, i) => (
            <Button
              key={i}
              color={value === pageSize ? 'red' : 'gray'}
              onClick={() => setParams({ pageSize: value })}
              className="focus:ring-0"
            >
              {value}
            </Button>
          ))}
        </Button.Group>
      </div>
    </div>
  )
}

export default Filters
