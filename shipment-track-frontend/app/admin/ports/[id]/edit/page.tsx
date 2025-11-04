"use client"

import { useState, useEffect } from "react"
import Link from "next/link"
import { useRouter, useParams } from "next/navigation"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { AlertCircle, ArrowLeft, Loader2 } from "lucide-react"
import { ApiError } from "@/lib/api/http"
import { getPortById, updatePort } from "@/lib/services/port.service"
import type { Port, UpdatePortPayload } from "@/lib/types/port"
import { useToast } from "@/hooks/use-toast"
import { getSession } from "@/lib/services/auth.service"

export default function EditPortPage() {
  const router = useRouter()
  const params = useParams()
  const { toast } = useToast()
  const [port, setPort] = useState<Port | null>(null)
  const [formData, setFormData] = useState<UpdatePortPayload>({
    name: "",
    country: "",
    city: "",
    isActive: true,
  })
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState("")
  const [authorized, setAuthorized] = useState(false)

  useEffect(() => {
    const session = getSession()
    const roles = Array.isArray(session?.claims?.roles) ? (session!.claims!.roles as string[]) : []
    setAuthorized(roles.includes("Admin"))

    const fetchPort = async () => {
      try {
        const id = params.id as string
        const data = await getPortById(id)
        setPort(data)
        setFormData({
          name: data.name,
          country: data.country,
          city: data.city || "",
          isActive: data.isActive,
        })
      } catch (err) {
        console.error("Failed to load port:", err)
        setError("Failed to load port details. Please try again.")
      } finally {
        setLoading(false)
      }
    }

    fetchPort()
  }, [params.id])

  const handleChange = (field: keyof UpdatePortPayload, value: string | boolean) => {
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

    if (!port) return

    setSaving(true)

    try {
      await updatePort(port.id, {
        name: formData.name.trim(),
        country: formData.country.trim(),
        city: formData.city?.trim() || null,
        isActive: formData.isActive,
      })

      toast({ description: "Port updated successfully." })
      router.push("/admin/ports")
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message || "Failed to update port. Please review your input.")
      } else {
        setError("Unexpected error while updating port.")
      }
    } finally {
      setSaving(false)
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
            <h1 className="text-3xl font-bold text-foreground">Edit Port</h1>
            <p className="text-destructive mt-1">You do not have permission to edit ports. Admin role required.</p>
          </div>
        </div>
      </div>
    )
  }

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (!port) {
    return (
      <div className="space-y-6">
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>Port not found.</AlertDescription>
        </Alert>
        <Link href="/admin/ports">
          <Button variant="outline">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Ports
          </Button>
        </Link>
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
          <h1 className="text-3xl font-bold text-foreground">Edit Port</h1>
          <p className="text-muted-foreground mt-1">Update port details</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Port Details</CardTitle>
          <CardDescription>
            Port code: <span className="font-mono font-semibold">{port.code}</span> (cannot be changed)
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

            <div className="flex items-center space-x-2">
              <Switch
                id="isActive"
                checked={formData.isActive}
                onCheckedChange={(checked) => handleChange("isActive", checked)}
              />
              <Label htmlFor="isActive" className="cursor-pointer">
                Active (available for selection in shipment forms)
              </Label>
            </div>

            {error && (
              <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>{error}</AlertDescription>
              </Alert>
            )}

            <div className="flex gap-4">
              <Button type="submit" disabled={saving}>
                {saving ? (
                  <>
                    <Loader2 className="h-4 w-4 animate-spin mr-2" />
                    Saving...
                  </>
                ) : (
                  "Save Changes"
                )}
              </Button>
              <Button
                type="button"
                variant="outline"
                onClick={() => router.push("/admin/ports")}
                disabled={saving}
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
