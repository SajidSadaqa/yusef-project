"use client"

import type React from "react"

import { useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { AlertCircle, Loader2, CheckCircle2, Check, X } from "lucide-react"
import Image from "next/image"
import { ApiError } from "@/lib/api/http"
import { register as registerUser } from "@/lib/services/auth.service"

type RegistrationForm = {
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  password: string
  confirmPassword: string
}

const initialFormState: RegistrationForm = {
  firstName: "",
  lastName: "",
  email: "",
  phoneNumber: "",
  password: "",
  confirmPassword: "",
}

export default function RegisterPage() {
  const router = useRouter()
  const [formData, setFormData] = useState<RegistrationForm>(initialFormState)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const [success, setSuccess] = useState(false)

  // Password validation helper
  const validatePassword = (password: string) => {
    const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/
    return passwordPattern.test(password)
  }

  const getPasswordRequirements = (password: string) => {
    return {
      minLength: password.length >= 8,
      hasUpperCase: /[A-Z]/.test(password),
      hasLowerCase: /[a-z]/.test(password),
      hasDigit: /\d/.test(password),
      hasSpecialChar: /[^\da-zA-Z]/.test(password),
    }
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }))
  }

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault()
    setError("")

    if (!formData.firstName || !formData.lastName || !formData.email || !formData.password || !formData.confirmPassword) {
      setError("Please fill in all required fields")
      return
    }

    if (formData.password !== formData.confirmPassword) {
      setError("Passwords do not match")
      return
    }

    if (!validatePassword(formData.password)) {
      setError("Password must be at least 8 characters and include upper, lower, digit, and special character.")
      return
    }

    setLoading(true)

    try {
      const origin = typeof window !== "undefined" ? window.location.origin : undefined
      await registerUser({
        email: formData.email,
        password: formData.password,
        firstName: formData.firstName.trim() || undefined,
        lastName: formData.lastName.trim() || undefined,
        phoneNumber: formData.phoneNumber.trim() || undefined,
        verificationCallbackUrl: origin ? `${origin}/verify-email` : undefined,
        dashboardUrl: origin ? `${origin}/admin` : undefined,
      })

      setSuccess(true)
      setTimeout(() => {
        router.push("/login")
      }, 2000)
    } catch (err) {
      if (err instanceof ApiError) {
        // Try to extract specific field errors from backend response
        const payload = err.payload as any
        if (payload?.errors) {
          // Get the first error message from the errors object
          const firstErrorKey = Object.keys(payload.errors)[0]
          const firstError = payload.errors[firstErrorKey]
          const message = Array.isArray(firstError) ? firstError[0] : firstError
          setError(message || err.message)
        } else {
          setError(err.message || "Unable to complete registration. Please verify your details.")
        }
      } else {
        setError("Unexpected error during registration. Please try again.")
      }
    } finally {
      setLoading(false)
    }
  }

  if (success) {
    return (
      <div className="container mx-auto px-4 py-12 animate-fade-in">
        <div className="max-w-md mx-auto">
          <Card className="animate-scale-in">
            <CardContent className="pt-6 text-center">
              <div className="h-16 w-16 rounded-full bg-green-100 flex items-center justify-center mx-auto mb-4 animate-bounce">
                <CheckCircle2 className="h-8 w-8 text-green-600" />
              </div>
              <h2 className="text-2xl font-bold mb-2 text-foreground">Account Created!</h2>
              <p className="text-muted-foreground mb-4">Redirecting you to login...</p>
            </CardContent>
          </Card>
        </div>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-12 animate-fade-in">
      <div className="max-w-md mx-auto">
        <div className="text-center mb-8 animate-slide-up">
          <Image
            src="/vertex-logo.png"
            alt="Vertex Transport"
            width={200}
            height={60}
            className="h-14 w-auto mx-auto mb-6"
          />
          <h1 className="text-3xl font-bold mb-2 text-foreground">Create Account</h1>
          <p className="text-muted-foreground">Sign up to start tracking your shipments</p>
        </div>

        <Card className="animate-scale-in animation-delay-200">
          <CardHeader>
            <CardTitle>Sign Up</CardTitle>
            <CardDescription>Create your Vertex Transport account</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleRegister} className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="firstName">First Name</Label>
                  <Input
                    id="firstName"
                    name="firstName"
                    type="text"
                    placeholder="Jane"
                    value={formData.firstName}
                    onChange={handleChange}
                    disabled={loading}
                    className="transition-all duration-200 focus:scale-[1.02]"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="lastName">Last Name</Label>
                  <Input
                    id="lastName"
                    name="lastName"
                    type="text"
                    placeholder="Doe"
                    value={formData.lastName}
                    onChange={handleChange}
                    disabled={loading}
                    className="transition-all duration-200 focus:scale-[1.02]"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  name="email"
                  type="email"
                  placeholder="you@example.com"
                  value={formData.email}
                  onChange={handleChange}
                  disabled={loading}
                  className="transition-all duration-200 focus:scale-[1.02]"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="phoneNumber">Phone Number (optional)</Label>
                <Input
                  id="phoneNumber"
                  name="phoneNumber"
                  type="tel"
                  placeholder="+1 (555) 123-4567"
                  value={formData.phoneNumber}
                  onChange={handleChange}
                  disabled={loading}
                  className="transition-all duration-200 focus:scale-[1.02]"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="password">Password</Label>
                <Input
                  id="password"
                  name="password"
                  type="password"
                  placeholder="********"
                  value={formData.password}
                  onChange={handleChange}
                  disabled={loading}
                  className="transition-all duration-200 focus:scale-[1.02]"
                />
                {formData.password && (
                  <div className="space-y-1.5 text-xs">
                    {(() => {
                      const requirements = getPasswordRequirements(formData.password)
                      return (
                        <>
                          <div className={`flex items-center gap-1.5 ${requirements.minLength ? "text-green-600" : "text-muted-foreground"}`}>
                            {requirements.minLength ? <Check className="h-3 w-3" /> : <X className="h-3 w-3" />}
                            <span>At least 8 characters</span>
                          </div>
                          <div className={`flex items-center gap-1.5 ${requirements.hasUpperCase ? "text-green-600" : "text-muted-foreground"}`}>
                            {requirements.hasUpperCase ? <Check className="h-3 w-3" /> : <X className="h-3 w-3" />}
                            <span>One uppercase letter</span>
                          </div>
                          <div className={`flex items-center gap-1.5 ${requirements.hasLowerCase ? "text-green-600" : "text-muted-foreground"}`}>
                            {requirements.hasLowerCase ? <Check className="h-3 w-3" /> : <X className="h-3 w-3" />}
                            <span>One lowercase letter</span>
                          </div>
                          <div className={`flex items-center gap-1.5 ${requirements.hasDigit ? "text-green-600" : "text-muted-foreground"}`}>
                            {requirements.hasDigit ? <Check className="h-3 w-3" /> : <X className="h-3 w-3" />}
                            <span>One number</span>
                          </div>
                          <div className={`flex items-center gap-1.5 ${requirements.hasSpecialChar ? "text-green-600" : "text-muted-foreground"}`}>
                            {requirements.hasSpecialChar ? <Check className="h-3 w-3" /> : <X className="h-3 w-3" />}
                            <span>One special character (!@#$%^&*)</span>
                          </div>
                        </>
                      )
                    })()}
                  </div>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="confirmPassword">Confirm Password</Label>
                <Input
                  id="confirmPassword"
                  name="confirmPassword"
                  type="password"
                  placeholder="********"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  disabled={loading}
                  className="transition-all duration-200 focus:scale-[1.02]"
                />
              </div>

              {error && (
                <Alert variant="destructive" className="animate-slide-in-left">
                  <AlertCircle className="h-4 w-4" />
                  <AlertDescription>{error}</AlertDescription>
                </Alert>
              )}

              <Button
                type="submit"
                className="w-full bg-primary hover:bg-primary/90 transition-all duration-200 hover:scale-[1.02] active:scale-95"
                disabled={loading}
              >
                {loading ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Creating account...
                  </>
                ) : (
                  "Create Account"
                )}
              </Button>
            </form>

            <div className="mt-6 text-center text-sm">
              <span className="text-muted-foreground">Already have an account? </span>
              <Link href="/login" className="text-primary hover:underline font-medium transition-all duration-200">
                Sign in
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
