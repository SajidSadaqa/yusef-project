"use client"

import Link from "next/link"
import { usePathname, useRouter } from "next/navigation"
import { LayoutDashboard, Package, Users, LogOut, Anchor } from "lucide-react"
import { cn } from "@/lib/utils"
import { getSession, logout } from "@/lib/services/auth.service"
import { useEffect, useMemo, useState } from "react"

type NavItem = { title: string; href: string; icon: React.ComponentType<{ className?: string }> }

const allNavItems: NavItem[] = [
  { title: "Dashboard", href: "/admin", icon: LayoutDashboard },
  { title: "Shipments", href: "/admin/shipments", icon: Package },
  { title: "Ports", href: "/admin/ports", icon: Anchor },
  { title: "Users", href: "/admin/users", icon: Users },
]

export function AdminNav() {
  const pathname = usePathname()
  const router = useRouter()
  const [isAdmin, setIsAdmin] = useState(false)

  useEffect(() => {
    const session = getSession()
    const roles = Array.isArray(session?.claims?.roles) ? (session!.claims!.roles as string[]) : []
    setIsAdmin(roles.includes("Admin"))
  }, [])

  const navItems = useMemo(() => {
    // Hide Dashboard, Ports, Users, Settings for non-admins.
    // Keep Shipments visible (Staff access shipments).
    if (isAdmin) return allNavItems
    return allNavItems.filter((item) => item.title === "Shipments")
  }, [isAdmin])

  return (
    <nav className="space-y-1">
      {navItems.map((item) => {
        const Icon = item.icon
        const isActive = pathname === item.href
        return (
          <Link
            key={item.href}
            href={item.href}
            className={cn(
              "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
              isActive
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:bg-muted hover:text-foreground",
            )}
          >
            <Icon className="h-4 w-4" />
            {item.title}
          </Link>
        )
      })}
      <button
        className="flex w-full items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-muted hover:text-foreground transition-colors"
        onClick={() => {
          logout()
          router.replace("/login")
        }}
      >
        <LogOut className="h-4 w-4" />
        Logout
      </button>
    </nav>
  )
}
