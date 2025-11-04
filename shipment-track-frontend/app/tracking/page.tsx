"use client"

import type React from "react"

import { useState } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Search, Package, MapPin, Clock, CheckCircle2, Truck, AlertCircle } from "lucide-react"
import { ApiError } from "@/lib/api/http"
import { trackShipmentPublic } from "@/lib/services/shipment.service"
import { ShipmentStatus, type PublicTracking } from "@/lib/types/shipment"

const statusMeta: Record<ShipmentStatus, { label: string; color: string; icon: React.ReactNode }> = {
  [ShipmentStatus.Received]: { label: "Received", color: "bg-gray-500", icon: <Package className="h-5 w-5" /> },
  [ShipmentStatus.Packed]: { label: "Packed", color: "bg-amber-500", icon: <Package className="h-5 w-5" /> },
  [ShipmentStatus.AtOriginPort]: { label: "At Origin Port", color: "bg-blue-500", icon: <MapPin className="h-5 w-5" /> },
  [ShipmentStatus.OnVessel]: { label: "In Transit", color: "bg-yellow-500", icon: <Truck className="h-5 w-5" /> },
  [ShipmentStatus.ArrivedToPort]: { label: "Arrived to Port", color: "bg-indigo-500", icon: <MapPin className="h-5 w-5" /> },
  [ShipmentStatus.CustomsCleared]: { label: "Customs Cleared", color: "bg-purple-500", icon: <Package className="h-5 w-5" /> },
  [ShipmentStatus.OutForDelivery]: { label: "Out for Delivery", color: "bg-blue-600", icon: <Truck className="h-5 w-5" /> },
  [ShipmentStatus.Delivered]: { label: "Delivered", color: "bg-green-600", icon: <CheckCircle2 className="h-5 w-5" /> },
  [ShipmentStatus.Returned]: { label: "Returned", color: "bg-red-500", icon: <Package className="h-5 w-5" /> },
  [ShipmentStatus.Cancelled]: { label: "Cancelled", color: "bg-gray-400", icon: <AlertCircle className="h-5 w-5" /> },
}

const formatDateTime = (value?: string | null) => {
  if (!value) {
    return "N/A"
  }

  try {
    return new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" }).format(new Date(value))
  } catch {
    return value
  }
}

const formatDateOnly = (value?: string | null) => {
  if (!value) {
    return "N/A"
  }

  try {
    return new Intl.DateTimeFormat(undefined, { dateStyle: "medium" }).format(new Date(value))
  } catch {
    return value
  }
}

export default function TrackingPage() {
  const [trackingNumber, setTrackingNumber] = useState("")
  const [result, setResult] = useState<PublicTracking | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")

  const handleTrack = async () => {
    if (!trackingNumber.trim()) {
      setError("Please enter a tracking number")
      return
    }

    setLoading(true)
    setError("")

    try {
      const data = await trackShipmentPublic(trackingNumber.trim())
      setResult(data)
    } catch (err) {
      setResult(null)
      if (err instanceof ApiError) {
        setError(err.message || "Shipment not found. Please verify the tracking number.")
      } else {
        setError("Unable to retrieve tracking information at this time.")
      }
    } finally {
      setLoading(false)
    }
  }

  const currentStatusMeta = result ? statusMeta[result.currentStatus] ?? statusMeta[ShipmentStatus.Received] : null

  return (
    <div className="container mx-auto px-4 py-12">
      <div className="max-w-4xl mx-auto">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold mb-4 text-foreground">Track Your Shipment</h1>
          <p className="text-muted-foreground text-lg leading-relaxed">
            Enter your tracking number to get real-time updates on your delivery
          </p>
        </div>

        <Card className="mb-8">
          <CardHeader>
            <CardTitle>Enter Tracking Number</CardTitle>
            <CardDescription>Your tracking number can be found in your confirmation email</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col sm:flex-row gap-3">
              <Input
                placeholder="e.g., VTX123456789"
                value={trackingNumber}
                onChange={(e) => {
                  setTrackingNumber(e.target.value)
                  setError("")
                }}
                onKeyDown={(e) => e.key === "Enter" && handleTrack()}
                className="flex-1"
                disabled={loading}
              />
              <Button onClick={handleTrack} disabled={loading} className="bg-primary hover:bg-primary/90">
                <Search className="h-4 w-4 mr-2" />
                {loading ? "Tracking..." : "Track Shipment"}
              </Button>
            </div>
            {error && (
              <div className="mt-4 flex items-center gap-2 text-sm text-destructive">
                <AlertCircle className="h-4 w-4" />
                {error}
              </div>
            )}
          </CardContent>
        </Card>

        {result && currentStatusMeta && (
          <div className="space-y-6 animate-fade-in">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center justify-between">
                  <span>Tracking Details</span>
                  <Badge className={`${currentStatusMeta.color} px-3 py-1 text-white`}>
                    <div className="flex items-center gap-2">
                      {currentStatusMeta.icon}
                      <span>{currentStatusMeta.label}</span>
                    </div>
                  </Badge>
                </CardTitle>
                <CardDescription>Tracking Number: {result.trackingNumber}</CardDescription>
              </CardHeader>
              <CardContent className="grid gap-6 md:grid-cols-2">
                <div className="space-y-3">
                  <div className="flex items-center gap-3 p-4 rounded-lg bg-muted/50">
                    <Package className="h-5 w-5 text-primary" />
                    <div>
                      <p className="text-sm text-muted-foreground">Current Status</p>
                      <p className="font-medium text-foreground">{currentStatusMeta.label}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3 p-4 rounded-lg bg-muted/50">
                    <MapPin className="h-5 w-5 text-accent" />
                    <div>
                      <p className="text-sm text-muted-foreground">Current Location</p>
                      <p className="font-medium text-foreground">{result.currentLocation ?? "N/A"}</p>
                    </div>
                  </div>
                </div>
                <div className="space-y-3">
                  <div className="flex items-center gap-3 p-4 rounded-lg bg-muted/50">
                    <Clock className="h-5 w-5 text-secondary" />
                    <div>
                      <p className="text-sm text-muted-foreground">Estimated Arrival</p>
                      <p className="font-medium text-foreground">{formatDateOnly(result.estimatedArrivalUtc)}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3 p-4 rounded-lg bg-muted/50">
                    <Package className="h-5 w-5 text-primary" />
                    <div>
                      <p className="text-sm text-muted-foreground">Latest Event</p>
                      <p className="font-medium text-foreground">
                        {result.statusHistory[0]?.description ?? currentStatusMeta.label}
                      </p>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Shipment Timeline</CardTitle>
                <CardDescription>Latest updates for your shipment</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {(result.statusHistory ?? []).length === 0 && (
                    <p className="text-sm text-muted-foreground">No tracking history is available yet.</p>
                  )}
                  {(result.statusHistory ?? []).map((update) => {
                    const meta = statusMeta[update.status] ?? statusMeta[ShipmentStatus.Received]
                    return (
                      <div
                        key={update.id}
                        className="flex items-start gap-4 p-4 rounded-lg border border-border hover:bg-muted/50 transition-colors"
                      >
                        <div className={`h-10 w-10 rounded-full ${meta.color} flex items-center justify-center text-white`}>
                          {meta.icon}
                        </div>
                        <div>
                          <div className="flex items-center gap-2">
                            <h3 className="font-semibold text-foreground">{meta.label}</h3>
                            {update.location && <Badge variant="outline">{update.location}</Badge>}
                          </div>
                          <p className="text-sm text-muted-foreground mt-1">{formatDateTime(update.eventTimeUtc)}</p>
                          {update.description && <p className="text-sm text-foreground mt-2">{update.description}</p>}
                        </div>
                      </div>
                    )
                  })}
                </div>
              </CardContent>
            </Card>
          </div>
        )}

        {!result && !loading && (
          <Card className="border-dashed border-2 border-muted mt-6">
            <CardContent className="text-center py-10">
              <Package className="h-12 w-12 mx-auto mb-4 text-muted-foreground" />
              <h3 className="text-xl font-semibold mb-2 text-foreground">Track Your Shipment</h3>
              <p className="text-muted-foreground">
                Enter a tracking number above to view shipment details and real-time updates.
              </p>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  )
}
