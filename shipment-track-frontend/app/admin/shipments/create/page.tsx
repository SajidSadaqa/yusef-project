"use client"

import { useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { AlertCircle, ArrowLeft, Loader2 } from "lucide-react"
import { ApiError } from "@/lib/api/http"
import { createShipment } from "@/lib/services/shipment.service"
import type { CreateShipmentPayload } from "@/lib/types/shipment"
import { useToast } from "@/hooks/use-toast"

const initialForm: CreateShipmentPayload = {
  referenceNumber: "",
  customerId: "",
  originPort: "",
  destinationPort: "",
  weightKg: 0,
  volumeCbm: 0,
  estimatedDepartureUtc: "",
  estimatedArrivalUtc: "",
  currentLocation: "",
  notes: "",
}

export default function CreateShipmentPage() {
  const router = useRouter()
  const { toast } = useToast()
  const [formData, setFormData] = useState(initialForm)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")

  const handleChange = (field: keyof CreateShipmentPayload, value: string) => {
    setFormData((prev) => ({
      ...prev,
      [field]:
        field === "weightKg" || field === "volumeCbm"
          ? value
            ? Number.parseFloat(value)
            : 0
          : value,
    }))
  }

  const toUtcIsoString = (value?: string | null) => {
    if (!value) {
      return undefined
    }

    const date = new Date(value)
    if (Number.isNaN(date.getTime())) {
      return undefined
    }

    return date.toISOString()
  }

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    setError("")

    if (!formData.referenceNumber || !formData.originPort || !formData.destinationPort) {
      setError("Reference number, origin port, and destination port are required.")
      return
    }

    if (!formData.weightKg || formData.weightKg <= 0) {
      setError("Weight must be greater than zero.")
      return
    }

    setLoading(true)

    try {
      await createShipment({
        ...formData,
        customerId: formData.customerId?.trim() ? formData.customerId : undefined,
        currentLocation: formData.currentLocation?.trim() || undefined,
        notes: formData.notes?.trim() || undefined,
        estimatedDepartureUtc: toUtcIsoString(formData.estimatedDepartureUtc) ?? undefined,
        estimatedArrivalUtc: toUtcIsoString(formData.estimatedArrivalUtc) ?? undefined,
      })

      toast({ description: "Shipment created successfully." })
      router.push("/admin/shipments")
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message || "Failed to create shipment. Please review your input.")
      } else {
        setError("Unexpected error while creating shipment.")
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin/shipments">
          <Button variant="ghost" size="sm">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-3xl font-bold text-foreground">Create Shipment</h1>
          <p className="text-muted-foreground mt-1">Provide the shipment details to generate a new tracking record.</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Shipment Details</CardTitle>
          <CardDescription>Required operational data for the new shipment.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="referenceNumber">Reference Number *</Label>
                <Input
                  id="referenceNumber"
                  value={formData.referenceNumber}
                  onChange={(e) => handleChange("referenceNumber", e.target.value)}
                  placeholder="REF-2025-0001"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="customerId">Customer ID</Label>
                <Input
                  id="customerId"
                  value={formData.customerId ?? ""}
                  onChange={(e) => handleChange("customerId", e.target.value)}
                  placeholder="Optional"
                />
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="originPort">Origin Port *</Label>
                <Input
                  id="originPort"
                  value={formData.originPort}
                  onChange={(e) => handleChange("originPort", e.target.value)}
                  placeholder="Los Angeles"
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="destinationPort">Destination Port *</Label>
                <Input
                  id="destinationPort"
                  value={formData.destinationPort}
                  onChange={(e) => handleChange("destinationPort", e.target.value)}
                  placeholder="Singapore"
                  required
                />
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="weightKg">Weight (kg) *</Label>
                <Input
                  id="weightKg"
                  type="number"
                  min="0"
                  step="0.01"
                  value={formData.weightKg || ""}
                  onChange={(e) => handleChange("weightKg", e.target.value)}
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="volumeCbm">Volume (cbm)</Label>
                <Input
                  id="volumeCbm"
                  type="number"
                  min="0"
                  step="0.01"
                  value={formData.volumeCbm || ""}
                  onChange={(e) => handleChange("volumeCbm", e.target.value)}
                />
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="estimatedDepartureUtc">Estimated Departure (UTC)</Label>
                <Input
                  id="estimatedDepartureUtc"
                  type="datetime-local"
                  value={formData.estimatedDepartureUtc ?? ""}
                  onChange={(e) => handleChange("estimatedDepartureUtc", e.target.value)}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="estimatedArrivalUtc">Estimated Arrival (UTC)</Label>
                <Input
                  id="estimatedArrivalUtc"
                  type="datetime-local"
                  value={formData.estimatedArrivalUtc ?? ""}
                  onChange={(e) => handleChange("estimatedArrivalUtc", e.target.value)}
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="currentLocation">Current Location</Label>
              <Input
                id="currentLocation"
                value={formData.currentLocation ?? ""}
                onChange={(e) => handleChange("currentLocation", e.target.value)}
                placeholder="Warehouse 12"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="notes">Notes</Label>
              <Textarea
                id="notes"
                value={formData.notes ?? ""}
                onChange={(e) => handleChange("notes", e.target.value)}
                placeholder="Additional handling instructions or comments"
                rows={4}
              />
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
                  "Create Shipment"
                )}
              </Button>
              <Button type="button" variant="outline" onClick={() => router.push("/admin/shipments")} disabled={loading}>
                Cancel
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
