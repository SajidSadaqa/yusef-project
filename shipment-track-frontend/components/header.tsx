"use client"

import Link from "next/link"
import Image from "next/image"
import { Button } from "@/components/ui/button"
import { Menu, X, LogOut } from "lucide-react"
import { useState, useEffect } from "react"
import { ThemeToggle } from "@/components/theme-toggle"
import { useRouter } from "next/navigation"
import { getSession, isAuthenticated, logout } from "@/lib/services/auth.service"

export function Header() {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)
  const [isLoggedIn, setIsLoggedIn] = useState(false)
  const [isAdmin, setIsAdmin] = useState(false)
  const router = useRouter()

  useEffect(() => {
    // Check auth state on mount
    setIsLoggedIn(isAuthenticated())
    const session = getSession()
    const roles = Array.isArray(session?.claims?.roles) ? (session!.claims!.roles as string[]) : []
    setIsAdmin(roles.includes("Admin"))

    // Listen for storage changes (login/logout events)
    const handleStorageChange = () => {
      setIsLoggedIn(isAuthenticated())
      const session = getSession()
      const roles = Array.isArray(session?.claims?.roles) ? (session!.claims!.roles as string[]) : []
      setIsAdmin(roles.includes("Admin"))
    }

    window.addEventListener("storage", handleStorageChange)

    // Also listen for custom login event
    window.addEventListener("login", handleStorageChange)
    window.addEventListener("logout", handleStorageChange)

    return () => {
      window.removeEventListener("storage", handleStorageChange)
      window.removeEventListener("login", handleStorageChange)
      window.removeEventListener("logout", handleStorageChange)
    }
  }, [])

  const handleLogout = () => {
    logout()
    setIsLoggedIn(false)
    router.push("/")
  }

  return (
    <header className="sticky top-0 z-50 w-full border-b border-border/40 bg-background/80 backdrop-blur-xl supports-backdrop-filter:bg-background/80 shadow-sm">
      <div className="container mx-auto flex h-20 items-center justify-between px-6">
        <Link href="/" className="flex items-center gap-3 group">
          <Image
            src="/vertex-logo.png"
            alt="Vertex Transport"
            width={200}
            height={70}
            className="h-14 w-auto transition-transform group-hover:scale-105"
          />
        </Link>

        {/* Desktop Navigation */}
        <nav className="hidden md:flex items-center gap-8">
          <Link
            href="/"
            className="text-sm font-semibold text-foreground hover:text-primary transition-all relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-primary after:transition-all hover:after:w-full"
          >
            Home
          </Link>
          <Link
            href="/services"
            className="text-sm font-semibold text-foreground hover:text-primary transition-all relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-primary after:transition-all hover:after:w-full"
          >
            Services
          </Link>
          <Link
            href="/about"
            className="text-sm font-semibold text-foreground hover:text-primary transition-all relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-primary after:transition-all hover:after:w-full"
          >
            About
          </Link>
          <Link
            href="/contact"
            className="text-sm font-semibold text-foreground hover:text-primary transition-all relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-primary after:transition-all hover:after:w-full"
          >
            Contact
          </Link>
        </nav>

        <div className="hidden md:flex items-center gap-4">
          <ThemeToggle />
          {isLoggedIn ? (
            <>
              {isAdmin && (
                <Link href="/admin">
                  <Button
                    variant="ghost"
                    size="default"
                    className="font-semibold hover:bg-primary/10 hover:text-primary transition-all"
                  >
                    Dashboard
                  </Button>
                </Link>
              )}
              <Button
                onClick={handleLogout}
                variant="outline"
                size="default"
                className="font-semibold hover:bg-destructive hover:text-destructive-foreground transition-all"
              >
                <LogOut className="mr-2 h-4 w-4" />
                Logout
              </Button>
            </>
          ) : (
            <>
              <Link href="/login">
                <Button
                  variant="ghost"
                  size="default"
                  className="font-semibold hover:bg-primary/10 hover:text-primary transition-all"
                >
                  Sign In
                </Button>
              </Link>
            </>
          )}
        </div>

        {/* Mobile Menu Button */}
        <button
          className="md:hidden p-2 hover:bg-muted rounded-lg transition-colors"
          onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
        >
          {mobileMenuOpen ? <X className="h-6 w-6" /> : <Menu className="h-6 w-6" />}
        </button>
      </div>

      {/* Mobile Menu */}
      {mobileMenuOpen && (
        <div className="md:hidden border-t border-border/40 bg-background/95 backdrop-blur-xl shadow-lg">
          <nav className="container mx-auto flex flex-col gap-2 p-6">
            <Link
              href="/"
              className="text-sm font-semibold text-foreground hover:text-primary hover:bg-primary/5 transition-all px-4 py-3 rounded-lg"
            >
              Home
            </Link>
            <Link
              href="/services"
              className="text-sm font-semibold text-foreground hover:text-primary hover:bg-primary/5 transition-all px-4 py-3 rounded-lg"
            >
              Services
            </Link>
            <Link
              href="/about"
              className="text-sm font-semibold text-foreground hover:text-primary hover:bg-primary/5 transition-all px-4 py-3 rounded-lg"
            >
              About
            </Link>
            <Link
              href="/contact"
              className="text-sm font-semibold text-foreground hover:text-primary hover:bg-primary/5 transition-all px-4 py-3 rounded-lg"
            >
              Contact
            </Link>
            <div className="flex flex-col gap-3 pt-4 mt-2 border-t border-border/40">
              <div className="flex items-center justify-between px-4 py-2">
                <span className="text-sm font-semibold">Theme</span>
                <ThemeToggle />
              </div>
              {isLoggedIn ? (
                <>
                  {isAdmin && (
                    <Link href="/admin">
                      <Button
                        variant="ghost"
                        size="default"
                        className="w-full font-semibold hover:bg-primary/10 hover:text-primary"
                      >
                        Dashboard
                      </Button>
                    </Link>
                  )}
                  <Button
                    onClick={handleLogout}
                    variant="outline"
                    size="default"
                    className="w-full font-semibold hover:bg-destructive hover:text-destructive-foreground"
                  >
                    <LogOut className="mr-2 h-4 w-4" />
                    Logout
                  </Button>
                </>
              ) : (
                <>
                  <Link href="/login">
                    <Button
                      variant="ghost"
                      size="default"
                      className="w-full font-semibold hover:bg-primary/10 hover:text-primary"
                    >
                      Sign In
                    </Button>
                  </Link>
                </>
              )}
            </div>
          </nav>
        </div>
      )}
    </header>
  )
}
