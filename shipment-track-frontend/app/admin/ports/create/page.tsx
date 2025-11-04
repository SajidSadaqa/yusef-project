"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { AlertCircle, ArrowLeft, Loader2 } from "lucide-react"
import { ApiError } from "@/lib/api/http"
import { createPort } from "@/lib/services/port.service"
import type { CreatePortPayload } from "@/lib/types/port"
import { useToast } from "@/hooks/use-toast"
import { getSession } from "@/lib/services/auth.service"

const initialForm: CreatePortPayload = {
  name: "",
  country: "",
  city: "",
  code: "",
}

export default function CreatePortPage() {
  const router = useRouter()
  const { toast } = useToast()
  const [formData, setFormData] = useState(initialForm)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const [authorized, setAuthorized] = useState(false)

  useEffect(() => {
    const session = getSession()
    const roles = Array.isArray(session?.claims?.roles) ? (session!.claims!.roles as string[]) : []
    setAuthorized(roles.includes("Admin"))
  }, [])

  const handleChange = (field: keyof CreatePortPayload, value: string) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }))
  }

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    setError("")

    if (!formData.name || !formData.country) {
      setError("Name and country are required.")
      return
    }

    setLoading(true)

    try {
      await createPort({
        name: formData.name.trim(),
        country: formData.country.trim(),
        city: formData.city?.trim() || null,
        code: formData.code?.trim() || null,
      })

      toast({ description: "Port created successfully." })
      router.push("/admin/ports")
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message || "Failed to create port. Please review your input.")
      } else {
        setError("Unexpected error while creating port.")
      }
    } finally {
      setLoading(false)
    }
  }

  if (!authorized) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Link href="/admin/ports">
            <Button variant="ghost" size="sm">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back
            </Button>
          </Link>
          <div>
            <h1 className="text-3xl font-bold text-foreground">Create Port</h1>
            <p className="text-destructive mt-1">You do not have permission to create ports. Admin role required.</p>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin/ports">
          <Button variant="ghost" size="sm">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-3xl font-bold text-foreground">Create Port</h1>
          <p className="text-muted-foreground mt-1">Add a new port to the system</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Port Details</CardTitle>
          <CardDescription>
            Port code will be auto-generated if not provided. Codes follow UN/LOCODE standard (5 characters).
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="name">Port Name *</Label>
              <Input
                id="name"
                value={formData.name}
                onChange={(e) => handleChange("name", e.target.value)}
                placeholder="Los Angeles"
                required
              />
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="country">Country *</Label>
                <Input
                  id="country"
                  value={formData.country}
                  onChange={(e) => handleChange("country", e.target.value)}
                  placeholder="United States"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="city">City</Label>
                <Input
                  id="city"
                  value={formData.city ?? ""}
                  onChange={(e) => handleChange("city", e.target.value)}
                  placeholder="Los Angeles"
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="code">Port Code (Optional)</Label>
              <Input
                id="code"
                value={formData.code ?? ""}
                onChange={(e) => handleChange("code", e.target.value)}
                placeholder="Auto-generated if empty"
                maxLength={5}
              />
              <p className="text-sm text-muted-foreground">
                Leave empty to auto-generate (e.g., USLOS, CNSHA). Max 5 characters.
              </p>
            </div>

            {error && (
              <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>{error}</AlertDescription>
              </Alert>
            )}

            <div className="flex gap-4">
              <Button type="submit" disabled={loading}>
                {loading ? (
                  <>
                    <Loader2 className="h-4 w-4 animate-spin mr-2" />
                    Creating...
                  </>
                ) : (
                  "Create Port"
                )}
              </Button>
              <Button
                type="button"
                variant="outline"
                onClick={() => router.push("/admin/ports")}
                disabled={loading}
              >
                Cancel
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
