"use client"

import type React from "react"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import { User } from "lucide-react"
import { AdminNav } from "@/components/admin-nav"
import { getSession } from "@/lib/services/auth.service"

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter()
  const [displayName, setDisplayName] = useState<string>("Admin Portal")
  const [authorized, setAuthorized] = useState(false)

  useEffect(() => {
    const session = getSession()
    const roles = Array.isArray(session?.claims?.roles) ? (session!.claims!.roles as string[]) : []

    if (!session || (!roles.includes("Admin") && !roles.includes("Staff"))) {
      router.replace("/login")
      return
    }

    const firstName = typeof session.claims?.firstName === "string" ? session.claims.firstName : ""
    const lastName = typeof session.claims?.lastName === "string" ? session.claims.lastName : ""
    const email = typeof session.claims?.email === "string" ? session.claims.email : ""

    const name = `${firstName} ${lastName}`.trim() || email || "Team Member"
    setDisplayName(name)
    setAuthorized(true)
  }, [router])

  if (!authorized) {
    return null
  }

  return (
    <div className="flex h-screen bg-background">
      <aside className="w-64 border-r border-border bg-muted/30 p-6">
        <div className="flex items-center gap-3 mb-8 pb-4 border-b border-border">
          <div className="h-10 w-10 rounded-full bg-primary flex items-center justify-center">
            <User className="h-5 w-5 text-primary-foreground" />
          </div>
          <div>
            <p className="font-semibold text-foreground">Admin Portal</p>
            <p className="text-sm text-muted-foreground truncate">{displayName}</p>
          </div>
        </div>
        <AdminNav />
      </aside>

      <main className="flex-1 overflow-y-auto">
        <div className="container mx-auto p-6">{children}</div>
      </main>
    </div>
  )
}
