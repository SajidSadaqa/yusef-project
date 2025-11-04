"use client"

import { useState, useEffect } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { AlertCircle, ArrowLeft, Loader2 } from "lucide-react"
import { ApiError } from "@/lib/api/http"
import { createShipment } from "@/lib/services/shipment.service"
import { listPorts } from "@/lib/services/port.service"
import type { CreateShipmentPayload } from "@/lib/types/shipment"
import type { Port } from "@/lib/types/port"
import { useToast } from "@/hooks/use-toast"

const initialForm: CreateShipmentPayload = {
  customerReference: "",
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
  const [ports, setPorts] = useState<Port[]>([])
  const [portsLoading, setPortsLoading] = useState(true)

  useEffect(() => {
    const fetchPorts = async () => {
      try {
        const data = await listPorts({ activeOnly: true })
        setPorts(data)
      } catch (err) {
        console.error("Failed to load ports:", err)
        setError("Failed to load ports. Please refresh the page.")
      } finally {
        setPortsLoading(false)
      }
    }
    fetchPorts()
  }, [])

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

    if (!formData.originPort || !formData.destinationPort) {
      setError("Origin and destination ports are required.")
      return
    }

    if (!formData.weightKg || formData.weightKg <= 0) {
      setError("Weight must be greater than zero.")
      return
    }

    setLoading(true)

    try {
      await createShipment({
        customerReference: formData.customerReference?.trim() || undefined,
        customerId: formData.customerId?.trim() ? formData.customerId : undefined,
        originPort: formData.originPort.trim(),
        destinationPort: formData.destinationPort.trim(),
        weightKg: formData.weightKg,
        volumeCbm: formData.volumeCbm,
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
          <CardDescription>
            Required operational data for the new shipment. Reference numbers are generated automatically.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="customerId">Customer ID</Label>
              <Input
                id="customerId"
                value={formData.customerId ?? ""}
                onChange={(e) => handleChange("customerId", e.target.value)}
                placeholder="Optional"
              />
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="originPort">Origin Port *</Label>
                <Select
                  value={formData.originPort}
                  onValueChange={(value) => handleChange("originPort", value)}
                  disabled={portsLoading}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={portsLoading ? "Loading ports..." : "Select origin port"} />
                  </SelectTrigger>
                  <SelectContent>
                    {ports.map((port) => (
                      <SelectItem key={port.id} value={port.code}>
                        {port.name} ({port.code}) - {port.country}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="destinationPort">Destination Port *</Label>
                <Select
                  value={formData.destinationPort}
                  onValueChange={(value) => handleChange("destinationPort", value)}
                  disabled={portsLoading}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={portsLoading ? "Loading ports..." : "Select destination port"} />
                  </SelectTrigger>
                  <SelectContent>
                    {ports.map((port) => (
                      <SelectItem key={port.id} value={port.code}>
                        {port.name} ({port.code}) - {port.country}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
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
