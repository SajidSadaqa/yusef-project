"use client"

import type React from "react"

import Link from "next/link"
import Image from "next/image"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogDescription,
} from "@/components/ui/dialog"
import { Badge } from "@/components/ui/badge"
import { Package, Truck, Clock, Shield, MapPin, BarChart3, Search, AlertCircle } from "lucide-react"
import { useState, useEffect } from "react"
import { ApiError } from "@/lib/api/http"
import { trackShipmentPublic } from "@/lib/services/shipment.service"
import { ShipmentStatus, type PublicTracking } from "@/lib/types/shipment"

const statusLabels: Record<ShipmentStatus, string> = {
  [ShipmentStatus.Received]: "Received",
  [ShipmentStatus.Packed]: "Packed",
  [ShipmentStatus.AtOriginPort]: "At Origin Port",
  [ShipmentStatus.OnVessel]: "In Transit",
  [ShipmentStatus.ArrivedToPort]: "Arrived to Port",
  [ShipmentStatus.CustomsCleared]: "Customs Cleared",
  [ShipmentStatus.OutForDelivery]: "Out for Delivery",
  [ShipmentStatus.Delivered]: "Delivered",
  [ShipmentStatus.Returned]: "Returned",
  [ShipmentStatus.Cancelled]: "Cancelled",
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

const isValidTrackingNumber = (value: string) => {
  return /^VTX-(\d{4})(0[1-9]|1[0-2])-(\d{4})$/.test(value.trim().toUpperCase())
}

export default function HomePage() {
  const [trackingNumber, setTrackingNumber] = useState("")
  const [showDetails, setShowDetails] = useState(false)
  const [shipmentData, setShipmentData] = useState<PublicTracking | null>(null)
  const [isVisible, setIsVisible] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")

  useEffect(() => {
    setIsVisible(true)
  }, [])

  const handleTrackShipment = async (e: React.FormEvent) => {
    e.preventDefault()
    const input = trackingNumber.trim().toUpperCase()

    if (!input) {
      setError("Please enter a tracking number")
      setShipmentData(null)
      return
    }

    if (!isValidTrackingNumber(input)) {
      setError("Invalid tracking number. Use VTX-YYYYMM-####.")
      setShipmentData(null)
      return
    }

    setLoading(true)
    setError("")

    try {
      const data = await trackShipmentPublic(input)
      setShipmentData(data)
      setShowDetails(true)
    } catch (err) {
      setShipmentData(null)
      if (err instanceof ApiError) {
        if (err.status === 404) {
          setError("Shipment not found. Please verify the tracking number.")
        } else if (err.status === 400) {
          setError("Invalid tracking number. Use VTX-YYYYMM-####.")
        } else {
          setError("Unable to retrieve tracking information at this time.")
        }
      } else {
        setError("Unable to retrieve tracking information at this time.")
      }
    } finally {
      setLoading(false)
    }
  }

  const historyEvents = shipmentData
    ? [...shipmentData.statusHistory].sort((a, b) => new Date(b.eventTimeUtc).getTime() - new Date(a.eventTimeUtc).getTime())
    : []

  const latestEvent = historyEvents[0]
  const statusLabel = shipmentData ? statusLabels[shipmentData.currentStatus] ?? "Unknown status" : ""
  const currentLocation = shipmentData ? shipmentData.currentLocation ?? latestEvent?.location ?? "N/A" : "N/A"

  return (
    <div className="flex flex-col relative">
      <section
        className={`py-10 sm:py-12 md:py-16 lg:py-20 bg-gradient-to-br from-primary/95 via-primary to-primary/90 transition-all duration-1000 ${isVisible ? "opacity-100 translate-y-0" : "opacity-0 translate-y-10"}`}
      >
        <div className="container mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 sm:gap-8 lg:gap-10 xl:gap-12 items-center">
            <div
              className={`order-2 lg:order-1 transition-all duration-1000 delay-200 ${isVisible ? "opacity-100 translate-x-0" : "opacity-0 -translate-x-10"}`}
            >
              <div className="mb-6 sm:mb-8">
                <h1 className="text-2xl sm:text-3xl md:text-4xl lg:text-5xl font-bold mb-2 sm:mb-3 text-primary-foreground leading-tight text-balance">
                  Track Your Shipment
                </h1>
                <p className="text-primary-foreground/90 text-sm sm:text-base md:text-lg leading-relaxed">
                  Enter your tracking number for real-time updates and precise location data
                </p>
              </div>
              <Card className="border-primary-foreground/20 bg-background/98 backdrop-blur shadow-xl hover:shadow-2xl transition-shadow duration-300">
                <CardContent className="p-4 sm:p-5 md:p-6">
                  <form onSubmit={handleTrackShipment} className="space-y-3 sm:space-y-4">
                      <div className="space-y-2">
                        <Label htmlFor="tracking" className="text-sm font-medium">
                          Tracking Number
                        </Label>
                        <div className="flex flex-col sm:flex-row gap-2 sm:gap-3">
                          <Input
                            id="tracking"
                            placeholder="e.g., VTX-202511-0001"
                            value={trackingNumber}
                            onChange={(e) => {
                              const value = e.target.value.toUpperCase()
                              setTrackingNumber(value)
                              setError("")
                            }}
                            className="flex-1 h-10 text-sm transition-all duration-200 focus:scale-[1.02]"
                            disabled={loading}
                          />
                          <Button
                            type="submit"
                            size="default"
                            className="h-10 px-5 bg-primary hover:bg-primary/90 w-full sm:w-auto text-sm transition-all duration-200 hover:scale-105"
                            disabled={loading}
                          >
                            <Search className={`h-4 w-4 mr-2 ${loading ? "animate-spin" : ""}`} />
                            {loading ? "Tracking..." : "Track"}
                          </Button>
                        </div>
                        <p className="text-xs text-muted-foreground">Format: VTX-YYYYMM-#### (e.g., VTX-202511-0001)</p>
                        {error && (
                          <div className="flex items-center gap-2 text-sm text-destructive">
                            <AlertCircle className="h-4 w-4" />
                            {error}
                          </div>
                        )}
                      </div>
                    </form>
                  </CardContent>
                </Card>
              </div>

            <div
              className={`relative h-[240px] sm:h-[300px] md:h-[350px] lg:h-[400px] overflow-hidden rounded-lg border-2 border-primary-foreground/20 shadow-2xl order-1 lg:order-2 transition-all duration-1000 delay-400 ${isVisible ? "opacity-100 translate-x-0" : "opacity-0 translate-x-10"}`}
            >
              <Image
                src="/truck-banner.avif"
                alt="Vertex Transport Truck"
                fill
                className="object-cover transition-transform duration-700 hover:scale-110"
                priority
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent" />
              <div className="absolute inset-0 flex items-end">
                <div className="p-4 sm:p-5 md:p-6 lg:p-8">
                  <h2 className="text-xl sm:text-2xl md:text-3xl lg:text-4xl font-bold text-white mb-1 text-balance leading-tight animate-fade-in">
                    EXPERT HAULING
                  </h2>
                  <h3 className="text-lg sm:text-xl md:text-2xl lg:text-3xl font-bold text-white text-balance animate-fade-in animation-delay-200">
                    SINCE 1990
                  </h3>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <Dialog open={showDetails} onOpenChange={setShowDetails}>
        <DialogContent className="max-w-[95vw] sm:max-w-[90vw] md:max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle className="text-xl sm:text-2xl font-bold bg-gradient-to-r from-primary via-accent to-secondary bg-clip-text text-transparent">
              Shipment Details
            </DialogTitle>
            <DialogDescription className="text-sm mt-2">
              Real-time tracking information for your shipment
            </DialogDescription>
          </DialogHeader>
          {shipmentData && (
            <div className="space-y-4 sm:space-y-5">
              <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 p-4 sm:p-5 bg-gradient-to-br from-primary/10 via-accent/5 to-secondary/10 rounded-lg border border-primary/20 hover:border-primary/40 transition-all hover:shadow-md">
                <div>
                  <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Tracking Number</p>
                  <p className="text-lg sm:text-xl font-bold text-foreground mt-1">{shipmentData.trackingNumber}</p>
                </div>
                <Badge className="text-sm px-3 py-1 bg-gradient-to-r from-primary to-accent text-white hover:shadow-md transition-shadow">
                  {statusLabel}
                </Badge>
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
                <div className="p-3 sm:p-4 rounded-lg bg-gradient-to-br from-blue-500/10 to-blue-500/5 border border-blue-500/20 hover:border-blue-500/40 transition-all">
                  <p className="text-xs font-semibold text-muted-foreground mb-1 uppercase tracking-wide">Current Status</p>
                  <p className="font-bold text-sm sm:text-base text-foreground">{statusLabel}</p>
                </div>
                <div className="p-3 sm:p-4 rounded-lg bg-gradient-to-br from-emerald-500/10 to-emerald-500/5 border border-emerald-500/20 hover:border-emerald-500/40 transition-all">
                  <p className="text-xs font-semibold text-muted-foreground mb-1 uppercase tracking-wide">
                    Current Location
                  </p>
                  <p className="font-bold text-sm sm:text-base text-foreground">{currentLocation}</p>
                </div>
                <div className="p-3 sm:p-4 rounded-lg bg-gradient-to-br from-amber-500/10 to-amber-500/5 border border-amber-500/20 hover:border-amber-500/40 transition-all">
                  <p className="text-xs font-semibold text-muted-foreground mb-1 uppercase tracking-wide">
                    Estimated Arrival
                  </p>
                  <p className="font-bold text-sm sm:text-base text-foreground">{formatDateTime(shipmentData.estimatedArrivalUtc)}</p>
                </div>
                <div className="p-3 sm:p-4 rounded-lg bg-gradient-to-br from-purple-500/10 to-purple-500/5 border border-purple-500/20 hover:border-purple-500/40 transition-all">
                  <p className="text-xs font-semibold text-muted-foreground mb-1 uppercase tracking-wide">
                    Latest Update
                  </p>
                  <p className="font-bold text-sm sm:text-base text-foreground">
                    {latestEvent ? formatDateTime(latestEvent.eventTimeUtc) : "N/A"}
                  </p>
                </div>
              </div>

              <div className="space-y-3">
                <h3 className="font-bold text-base sm:text-lg text-foreground flex items-center gap-2">
                  <Clock className="h-4 w-4 text-primary" />
                  Shipment History
                </h3>
                {historyEvents.length === 0 ? (
                  <p className="text-sm text-muted-foreground">No history recorded yet for this shipment.</p>
                ) : (
                  <div className="space-y-2 pl-3 border-l-2 border-border/50 last:border-0 group hover:translate-x-1 transition-transform">
                    {historyEvents.map((event) => {
                      const label = statusLabels[event.status] ?? "Unknown status"
                      return (
                        <div
                          key={event.id}
                          className="flex gap-3 pb-3 border-b border-border/50 last:border-0 group hover:translate-x-1 transition-transform"
                        >
                          <div className="flex-shrink-0 w-2 h-2 rounded-full bg-primary mt-1.5 ring-2 ring-primary/20 group-hover:ring-primary/40 transition-all" />
                          <div className="flex-1 p-3 rounded-lg bg-muted/30 hover:bg-muted/60 transition-colors">
                            <p className="font-bold text-sm text-foreground">{label}</p>
                            {event.description && (
                              <p className="text-xs text-foreground/80 mt-1.5 leading-relaxed">{event.description}</p>
                            )}
                            <p className="text-xs text-muted-foreground mt-1.5 flex items-center gap-1.5">
                              <MapPin className="h-3 w-3" />
                              {event.location ?? "N/A"}
                            </p>
                            <p className="text-xs text-muted-foreground mt-1 flex items-center gap-1.5">
                              <Clock className="h-3 w-3" />
                              {formatDateTime(event.eventTimeUtc)}
                            </p>
                          </div>
                        </div>
                      )
                    })}
                  </div>
                )}
              </div>
            </div>
          )}
          <DialogFooter>
            <Button
              onClick={() => setShowDetails(false)}
              className="bg-primary hover:bg-primary/90 px-5 w-full sm:w-auto text-sm"
            >
              Close
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <section
        className={`py-12 sm:py-14 md:py-16 lg:py-20 bg-background transition-all duration-1000 delay-600 ${isVisible ? "opacity-100" : "opacity-0"}`}
      >
        <div className="container mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-10 sm:mb-12">
            <h2 className="text-2xl sm:text-3xl md:text-4xl font-bold mb-3 sm:mb-4 text-foreground text-balance">
              Why Choose Vertex Transport
            </h2>
            <p className="text-muted-foreground text-sm sm:text-base md:text-lg max-w-3xl mx-auto leading-relaxed px-4">
              Industry-leading logistics solutions with cutting-edge tracking technology
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5 sm:gap-6">
            <Card className="border-border hover:shadow-lg transition-all duration-300 hover:-translate-y-2 animate-slide-up animation-delay-100">
              <CardContent className="p-5 sm:p-6">
                <div className="h-11 w-11 rounded-lg bg-primary/10 flex items-center justify-center mb-4 transition-transform duration-300 hover:scale-110 hover:rotate-6">
                  <Clock className="h-5 w-5 text-primary" />
                </div>
                <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Real-Time Tracking</h3>
                <p className="text-muted-foreground leading-relaxed text-sm">
                  Monitor your shipments 24/7 with live updates and precise location data
                </p>
              </CardContent>
            </Card>

            <Card className="border-border hover:shadow-lg transition-all duration-300 hover:-translate-y-2 animate-slide-up animation-delay-200">
              <CardContent className="p-5 sm:p-6">
                <div className="h-11 w-11 rounded-lg bg-secondary/10 flex items-center justify-center mb-4 transition-transform duration-300 hover:scale-110 hover:rotate-6">
                  <Shield className="h-5 w-5 text-secondary" />
                </div>
                <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Secure Delivery</h3>
                <p className="text-muted-foreground leading-relaxed text-sm">
                  Your cargo is protected with comprehensive insurance and security protocols
                </p>
              </CardContent>
            </Card>

            <Card className="border-border hover:shadow-lg transition-all duration-300 hover:-translate-y-2 animate-slide-up animation-delay-300">
              <CardContent className="p-5 sm:p-6">
                <div className="h-11 w-11 rounded-lg bg-accent/10 flex items-center justify-center mb-4 transition-transform duration-300 hover:scale-110 hover:rotate-6">
                  <Truck className="h-5 w-5 text-accent" />
                </div>
                <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Fleet Excellence</h3>
                <p className="text-muted-foreground leading-relaxed text-sm">
                  Modern, well-maintained vehicles ensuring reliable and timely deliveries
                </p>
              </CardContent>
            </Card>

            <Card className="border-border hover:shadow-lg transition-all duration-300 hover:-translate-y-2 animate-slide-up animation-delay-400">
              <CardContent className="p-5 sm:p-6">
                <div className="h-11 w-11 rounded-lg bg-primary/10 flex items-center justify-center mb-4 transition-transform duration-300 hover:scale-110 hover:rotate-6">
                  <MapPin className="h-5 w-5 text-primary" />
                </div>
                <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Wide Coverage</h3>
                <p className="text-muted-foreground leading-relaxed text-sm">
                  Extensive network covering California and major routes nationwide
                </p>
              </CardContent>
            </Card>

            <Card className="border-border hover:shadow-lg transition-all duration-300 hover:-translate-y-2 animate-slide-up animation-delay-500">
              <CardContent className="p-5 sm:p-6">
                <div className="h-11 w-11 rounded-lg bg-secondary/10 flex items-center justify-center mb-4 transition-transform duration-300 hover:scale-110 hover:rotate-6">
                  <BarChart3 className="h-5 w-5 text-secondary" />
                </div>
                <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Analytics Dashboard</h3>
                <p className="text-muted-foreground leading-relaxed text-sm">
                  Comprehensive reporting and insights for business customers
                </p>
              </CardContent>
            </Card>

            <Card className="border-border hover:shadow-lg transition-all duration-300 hover:-translate-y-2 animate-slide-up animation-delay-600">
              <CardContent className="p-5 sm:p-6">
                <div className="h-11 w-11 rounded-lg bg-accent/10 flex items-center justify-center mb-4 transition-transform duration-300 hover:scale-110 hover:rotate-6">
                  <Package className="h-5 w-5 text-accent" />
                </div>
                <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Flexible Solutions</h3>
                <p className="text-muted-foreground leading-relaxed text-sm">
                  Customized logistics services tailored to your specific needs
                </p>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>

      <section
        className={`py-12 sm:py-14 md:py-16 lg:py-20 bg-muted/30 transition-all duration-1000 delay-800 ${isVisible ? "opacity-100 scale-100" : "opacity-0 scale-95"}`}
      >
        <div className="container mx-auto px-4 sm:px-6 lg:px-8">
          <Card className="border-primary/20 bg-gradient-to-br from-primary/5 to-accent/5 shadow-lg hover:shadow-xl transition-all duration-300">
            <CardContent className="p-6 sm:p-8 md:p-10 lg:p-12 text-center">
              <h2 className="text-2xl sm:text-3xl md:text-4xl lg:text-5xl font-bold mb-3 sm:mb-4 text-foreground text-balance">
                Ready to Ship?
              </h2>
              <p className="text-muted-foreground text-sm sm:text-base md:text-lg mb-6 sm:mb-8 max-w-3xl mx-auto leading-relaxed px-4">
                Get started with Vertex Transport today and experience professional logistics services
              </p>
              <div className="flex flex-col sm:flex-row gap-3 sm:gap-4 justify-center items-center">
                <Link href="/login" className="w-full sm:w-auto">
                  <Button
                    size="lg"
                    variant="outline"
                    className="w-full sm:w-auto bg-transparent h-11 px-6 text-sm sm:text-base transition-all duration-200 hover:scale-105"
                  >
                    Customer Portal
                  </Button>
                </Link>
              </div>
            </CardContent>
          </Card>
        </div>
      </section>
    </div>
  )
}
