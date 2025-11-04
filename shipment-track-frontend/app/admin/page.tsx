"use client"

import { useEffect, useMemo, useState } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { AlertCircle, CheckCircle2, Clock, Package } from "lucide-react"
import { ApiError } from "@/lib/api/http"
import { listShipments } from "@/lib/services/shipment.service"
import type { Shipment } from "@/lib/types/shipment"

const STATUS_NAMES = [
  "Received",
  "Packed",
  "AtOriginPort",
  "OnVessel",
  "ArrivedToPort",
  "CustomsCleared",
  "OutForDelivery",
  "Delivered",
  "Returned",
  "Cancelled",
] as const

const statusLabel = (status: number) =>
  STATUS_NAMES[status]?.replace(/([A-Z])/g, " $1").trim() ?? `Status ${status}`

const formatDate = (value?: string | null) => {
  if (!value) {
    return "—"
  }

  try {
    return new Intl.DateTimeFormat(undefined, { dateStyle: "medium" }).format(new Date(value))
  } catch {
    return value
  }
}

export default function AdminDashboardPage() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const [totalShipments, setTotalShipments] = useState(0)
  const [inTransitCount, setInTransitCount] = useState(0)
  const [outForDeliveryCount, setOutForDeliveryCount] = useState(0)
  const [deliveredCount, setDeliveredCount] = useState(0)
  const [recentShipments, setRecentShipments] = useState<Shipment[]>([])

  useEffect(() => {
    const loadDashboard = async () => {
      setLoading(true)
      setError("")

      try {
        const [summary, inTransit, outForDelivery, delivered] = await Promise.all([
          listShipments({ page: 1, pageSize: 5 }),
          listShipments({ page: 1, pageSize: 1, status: "OnVessel" }),
          listShipments({ page: 1, pageSize: 1, status: "OutForDelivery" }),
          listShipments({ page: 1, pageSize: 1, status: "Delivered" }),
        ])

        // All API calls succeeded, update state with the data (even if empty)
        setTotalShipments(summary.totalCount)
        setRecentShipments(summary.items || [])
        setInTransitCount(inTransit.totalCount)
        setOutForDeliveryCount(outForDelivery.totalCount)
        setDeliveredCount(delivered.totalCount)
      } catch (err) {
        console.error("Dashboard loading error:", err)
        if (err instanceof ApiError) {
          setError(err.message || "Failed to load dashboard data.")
        } else {
          setError("Unexpected error while loading dashboard data.")
        }
      } finally {
        setLoading(false)
      }
    }

    loadDashboard()
  }, [])

  const stats = useMemo(
    () => [
      {
        title: "Total Shipments",
        value: totalShipments.toLocaleString(),
        icon: Package,
        helper: "All-time",
      },
      {
        title: "In Transit",
        value: inTransitCount.toLocaleString(),
        icon: Clock,
        helper: "Currently on vessel",
      },
      {
        title: "Out for Delivery",
        value: outForDeliveryCount.toLocaleString(),
        icon: Clock,
        helper: "Out for delivery today",
      },
      {
        title: "Delivered",
        value: deliveredCount.toLocaleString(),
        icon: CheckCircle2,
        helper: "Delivered shipments",
      },
    ],
    [deliveredCount, inTransitCount, outForDeliveryCount, totalShipments],
  )

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-foreground">Dashboard</h1>
        <p className="text-muted-foreground mt-1">Current overview of shipment operations.</p>
      </div>

      {error && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat) => {
          const Icon = stat.icon
          return (
            <Card key={stat.title}>
              <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">{stat.title}</CardTitle>
                <div className="h-8 w-8 rounded-lg bg-primary/10 flex items-center justify-center">
                  <Icon className="h-4 w-4 text-primary" />
                </div>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold text-foreground">{loading ? "…" : stat.value}</div>
                <p className="text-xs text-muted-foreground mt-1">{stat.helper}</p>
              </CardContent>
            </Card>
          )
        })}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Recent Shipments</CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <p className="text-sm text-muted-foreground">Loading recent shipments…</p>
          ) : recentShipments.length === 0 ? (
            <p className="text-sm text-muted-foreground">No shipments found yet.</p>
          ) : (
            <div className="space-y-4">
              {recentShipments.map((shipment) => (
                <div key={shipment.id} className="flex items-center justify-between p-4 border border-border rounded-lg">
                  <div className="flex-1">
                    <div className="font-semibold text-foreground">{shipment.trackingNumber}</div>
                    <div className="text-sm text-muted-foreground mt-1">
                      {shipment.originPort} → {shipment.destinationPort}
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="text-sm font-medium text-foreground">
                      ETA: {formatDate(shipment.estimatedArrivalUtc)}
                    </div>
                    <div className="text-xs text-muted-foreground mt-1">{statusLabel(shipment.currentStatus)}</div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
