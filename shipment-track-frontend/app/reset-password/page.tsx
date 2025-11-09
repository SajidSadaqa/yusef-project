"use client"

import { useState, useEffect } from "react"
import { useSearchParams, useRouter } from "next/navigation"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Loader2, CheckCircle2, XCircle } from "lucide-react"
import { resetPassword } from "@/lib/services/auth.service"
import { ApiError } from "@/lib/api/http"

export default function ResetPasswordPage() {
  const searchParams = useSearchParams()
  const router = useRouter()

  const [userId, setUserId] = useState("")
  const [token, setToken] = useState("")
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")

  const [status, setStatus] = useState<"form" | "submitting" | "success" | "error">("form")
  const [errorMessage, setErrorMessage] = useState("")

  useEffect(() => {
    const userIdParam = searchParams.get("userId")
    const tokenParam = searchParams.get("token")

    if (!userIdParam || !tokenParam) {
      setStatus("error")
      setErrorMessage("Invalid reset link. Please request a new password reset.")
      return
    }

    setUserId(userIdParam)
    setToken(tokenParam)
  }, [searchParams])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    // Validation
    if (!password || !confirmPassword) {
      setErrorMessage("Please fill in all fields")
      return
    }

    if (password.length < 8) {
      setErrorMessage("Password must be at least 8 characters long")
      return
    }

    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match")
      return
    }

    setStatus("submitting")
    setErrorMessage("")

    try {
      await resetPassword({ userId, token, newPassword: password })
      setStatus("success")
    } catch (error) {
      setStatus("error")
      if (error instanceof ApiError) {
        setErrorMessage(error.message)
      } else {
        setErrorMessage("An unexpected error occurred. Please try again.")
      }
    }
  }

  const handleContinue = () => {
    router.push("/login")
  }

  if (status === "success") {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary/10 via-background to-secondary/10 p-4">
        <Card className="w-full max-w-md">
          <CardHeader className="text-center">
            <CardTitle className="text-2xl">Password Reset Successful</CardTitle>
            <CardDescription>Your password has been reset successfully</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col items-center justify-center space-y-4 py-6">
              <CheckCircle2 className="h-16 w-16 text-green-500" />
              <div className="text-center space-y-2">
                <p className="text-lg font-medium">All Set!</p>
                <p className="text-sm text-muted-foreground">
                  Your password has been reset. You can now log in with your new password.
                </p>
              </div>
              <Button onClick={handleContinue} className="mt-4 w-full">
                Continue to Login
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (status === "error" && !userId) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary/10 via-background to-secondary/10 p-4">
        <Card className="w-full max-w-md">
          <CardHeader className="text-center">
            <CardTitle className="text-2xl">Invalid Reset Link</CardTitle>
            <CardDescription>This password reset link is invalid</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col items-center justify-center space-y-4 py-6">
              <XCircle className="h-16 w-16 text-destructive" />
              <div className="text-center space-y-2">
                <p className="text-lg font-medium">Link Invalid</p>
                <p className="text-sm text-muted-foreground">{errorMessage}</p>
              </div>
              <div className="flex flex-col gap-2 mt-4 w-full">
                <Button onClick={() => router.push("/forgot-password")} variant="outline" className="w-full">
                  Request New Reset Link
                </Button>
                <Button onClick={handleContinue} variant="ghost" className="w-full">
                  Go to Login
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary/10 via-background to-secondary/10 p-4">
      <Card className="w-full max-w-md">
        <CardHeader className="text-center">
          <CardTitle className="text-2xl">Reset Your Password</CardTitle>
          <CardDescription>Enter your new password below</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="password">New Password</Label>
              <Input
                id="password"
                type="password"
                placeholder="Enter new password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={status === "submitting"}
                required
              />
              <p className="text-xs text-muted-foreground">Must be at least 8 characters</p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="confirmPassword">Confirm New Password</Label>
              <Input
                id="confirmPassword"
                type="password"
                placeholder="Confirm new password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                disabled={status === "submitting"}
                required
              />
            </div>

            {errorMessage && (
              <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-md">
                <p className="text-sm text-destructive">{errorMessage}</p>
              </div>
            )}

            <Button type="submit" className="w-full" disabled={status === "submitting"}>
              {status === "submitting" && <Loader2 className="h-4 w-4 mr-2 animate-spin" />}
              {status === "submitting" ? "Resetting Password..." : "Reset Password"}
            </Button>

            <div className="text-center">
              <Button type="button" variant="link" onClick={handleContinue} className="text-sm">
                Back to Login
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
