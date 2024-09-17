'use client'

import Countdown, { zeroPad } from 'react-countdown'

type RendererProps = {
  days: number
  hours: number
  minutes: number
  seconds: number
  completed: boolean
}

// Renderer callback with condition
const renderer = ({ days, hours, minutes, seconds, completed }: RendererProps) => {
  return (
    <div
      className={`
            border-2 border-white text-white py-1 px-2 rounded-lg 
            flex justify-center 
            ${
              completed ? 'bg-red-600' : days === 0 && hours < 10 ? 'bg-amber-600' : 'bg-green-600'
            }`}
    >
      {completed ? (
        <span>Auction finished</span>
      ) : (
        <span suppressHydrationWarning={true}>
          {zeroPad(days)}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}
        </span>
      )}
    </div>
  )
}

type CountdownTimerProps = {
  auctionEnd: string
}

const CountdownTimer = ({ auctionEnd }: CountdownTimerProps) => {
  return <Countdown date={auctionEnd} renderer={renderer} />
}

export default CountdownTimer
