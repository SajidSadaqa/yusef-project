"use client"

import { Moon, Sun } from "lucide-react"
import { useEffect, useState } from "react"
import { Button } from "@/components/ui/button"

export function ThemeToggle() {
  const [theme, setTheme] = useState<"light" | "dark">("light")
  const [isAnimating, setIsAnimating] = useState(false)

  useEffect(() => {
    const savedTheme = localStorage.getItem("theme") as "light" | "dark" | null
    const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches

    const initialTheme = savedTheme || (prefersDark ? "dark" : "light")
    setTheme(initialTheme)
    document.documentElement.classList.toggle("dark", initialTheme === "dark")
  }, [])

  const toggleTheme = () => {
    setIsAnimating(true)

    const newTheme = theme === "light" ? "dark" : "light"
    setTheme(newTheme)
    localStorage.setItem("theme", newTheme)

    document.documentElement.style.transition = "background-color 0.3s ease, color 0.3s ease"
    document.documentElement.classList.toggle("dark", newTheme === "dark")

    setTimeout(() => {
      setIsAnimating(false)
      document.documentElement.style.transition = ""
    }, 300)
  }

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={toggleTheme}
      aria-label="Toggle theme"
      className="transition-transform duration-200 hover:scale-110 active:scale-95"
    >
      <div className={`transition-all duration-300 ${isAnimating ? "rotate-180 scale-0" : "rotate-0 scale-100"}`}>
        {theme === "light" ? <Moon className="h-5 w-5" /> : <Sun className="h-5 w-5" />}
      </div>
    </Button>
  )
}
