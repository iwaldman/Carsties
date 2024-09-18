'use client'

import { Button } from 'flowbite-react'
import { User } from 'next-auth'
import Link from 'next/link'

type Props = {
  user: User
}

export default function UserActions({ user }: Props) {
  return (
    <Button outline>
      <Link href='/session'>Session</Link>
    </Button>
  )
}
