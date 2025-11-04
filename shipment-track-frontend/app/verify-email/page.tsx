"use client"

import { useEffect, useState } from "react"
import { useSearchParams, useRouter } from "next/navigation"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Loader2, CheckCircle2, XCircle } from "lucide-react"
import { verifyEmail } from "@/lib/services/auth.service"
import { ApiError } from "@/lib/api/http"

export default function VerifyEmailPage() {
  const searchParams = useSearchParams()
  const router = useRouter()
  const [status, setStatus] = useState<"loading" | "success" | "error">("loading")
  const [errorMessage, setErrorMessage] = useState("")

  useEffect(() => {
    const userId = searchParams.get("userId")
    const token = searchParams.get("token")

    if (!userId || !token) {
      setStatus("error")
      setErrorMessage("Invalid verification link. Please check your email and try again.")
      return
    }

    verifyEmailToken(userId, token)
  }, [searchParams])

  const verifyEmailToken = async (userId: string, token: string) => {
    try {
      await verifyEmail(userId, token)
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

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary/10 via-background to-secondary/10 p-4">
      <Card className="w-full max-w-md">
        <CardHeader className="text-center">
          <CardTitle className="text-2xl">Email Verification</CardTitle>
          <CardDescription>
            {status === "loading" && "Verifying your email address..."}
            {status === "success" && "Your email has been verified successfully"}
            {status === "error" && "Verification failed"}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col items-center justify-center space-y-4 py-6">
            {status === "loading" && (
              <>
                <Loader2 className="h-16 w-16 animate-spin text-primary" />
                <p className="text-sm text-muted-foreground">Please wait while we verify your email...</p>
              </>
            )}

            {status === "success" && (
              <>
                <CheckCircle2 className="h-16 w-16 text-green-500" />
                <div className="text-center space-y-2">
                  <p className="text-lg font-medium">Verification Successful!</p>
                  <p className="text-sm text-muted-foreground">
                    Your email address has been verified. You can now log in to your account.
                  </p>
                </div>
                <Button onClick={handleContinue} className="mt-4 w-full">
                  Continue to Login
                </Button>
              </>
            )}

            {status === "error" && (
              <>
                <XCircle className="h-16 w-16 text-destructive" />
                <div className="text-center space-y-2">
                  <p className="text-lg font-medium">Verification Failed</p>
                  <p className="text-sm text-muted-foreground">{errorMessage}</p>
                </div>
                <div className="flex flex-col gap-2 mt-4 w-full">
                  <Button onClick={handleContinue} variant="outline" className="w-full">
                    Go to Login
                  </Button>
                </div>
              </>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
