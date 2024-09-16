import { Button, ButtonGroup } from 'flowbite-react'

const pageSizeButtons = [4, 8, 12]

type FiltersProps = {
  pageSize: number
  setPageSize: (pageSize: number) => void
}

export default function Filters({ pageSize, setPageSize }: FiltersProps) {
  return (
    <div className='flex justify-between items-center mb-4'>
      <div>
        <span className='uppercase text-sm text-gray-500 mr-2'>Page Size</span>
        <ButtonGroup>
          {pageSizeButtons.map((value, i) => (
            <Button
              key={i}
              onClick={() => setPageSize(value)}
              color={`${pageSize === value ? 'red' : 'gray'}`}
              className='focus:ring-0'
            >
              {value}
            </Button>
          ))}
        </ButtonGroup>
      </div>
    </div>
  )
}
