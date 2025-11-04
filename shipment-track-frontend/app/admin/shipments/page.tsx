"use client"

import { useEffect, useMemo, useState } from "react"
import Link from "next/link"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Textarea } from "@/components/ui/textarea"
import { Badge } from "@/components/ui/badge"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { useToast } from "@/hooks/use-toast"
import {
  AlertCircle,
  Package,
  CheckCircle2,
  History,
  Loader2,
  MapPin,
  Pencil,
  Plus,
  PlusCircle,
  RefreshCw,
  TrendingUp,
  Trash2,
  Truck,
} from "lucide-react"
import { ApiError } from "@/lib/api/http"
import {
  appendShipmentStatus,
  deleteShipment,
  getShipmentHistory,
  listShipments,
  updateShipment,
} from "@/lib/services/shipment.service"
import {
  ShipmentStatus,
  type Shipment,
  type ShipmentStatusHistory,
} from "@/lib/types/shipment"

const statusOptions = [
  { label: "All statuses", value: "all" as const },
  { label: "Received", value: "Received" as const },
  { label: "Packed", value: "Packed" as const },
  { label: "At Origin Port", value: "AtOriginPort" as const },
  { label: "On Vessel", value: "OnVessel" as const },
  { label: "Arrived to Port", value: "ArrivedToPort" as const },
  { label: "Customs Cleared", value: "CustomsCleared" as const },
  { label: "Out for Delivery", value: "OutForDelivery" as const },
  { label: "Delivered", value: "Delivered" as const },
  { label: "Returned", value: "Returned" as const },
  { label: "Cancelled", value: "Cancelled" as const },
] as const

type StatusFilter = (typeof statusOptions)[number]["value"]

type EditShipmentForm = {
  referenceNumber: string
  customerReference: string
  customerId: string
  originPort: string
  destinationPort: string
  weightKg: string
  volumeCbm: string
  estimatedDepartureUtc: string
  estimatedArrivalUtc: string
  currentLocation: string
  notes: string
}

type StatusUpdateForm = {
  status: ShipmentStatus
  description: string
  location: string
  eventTimeLocal: string
}

const statusKeyMap: Record<Exclude<StatusFilter, "all">, ShipmentStatus> = {
  Received: ShipmentStatus.Received,
  Packed: ShipmentStatus.Packed,
  AtOriginPort: ShipmentStatus.AtOriginPort,
  OnVessel: ShipmentStatus.OnVessel,
  ArrivedToPort: ShipmentStatus.ArrivedToPort,
  CustomsCleared: ShipmentStatus.CustomsCleared,
  OutForDelivery: ShipmentStatus.OutForDelivery,
  Delivered: ShipmentStatus.Delivered,
  Returned: ShipmentStatus.Returned,
  Cancelled: ShipmentStatus.Cancelled,
}

const statusLabel = (status: ShipmentStatus) => {
  for (const option of statusOptions) {
    if (option.value !== "all" && statusKeyMap[option.value] === status) {
      return option.label
    }
  }

  return ShipmentStatus[status]?.replace(/([A-Z])/g, " $1").trim() ?? `Status ${status}`
}

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

const formatDateTime = (value?: string | null) => {
  if (!value) {
    return "—"
  }

  try {
    return new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" }).format(new Date(value))
  } catch {
    return value
  }
}

const formatNumber = (value: number) => new Intl.NumberFormat().format(value)

const toInputDateTime = (value?: string | null) => {
  if (!value) {
    return ""
  }

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return ""
  }

  const offsetDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000)
  return offsetDate.toISOString().slice(0, 16)
}

const toUtcIsoString = (value: string) => {
  if (!value) {
    return undefined
  }

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return undefined
  }

  return date.toISOString()
}

const createStatusForm = (shipment?: Shipment): StatusUpdateForm => ({
  status: shipment?.currentStatus ?? ShipmentStatus.Received,
  description: "",
  location: shipment?.currentLocation ?? "",
  eventTimeLocal: toInputDateTime(new Date().toISOString()),
})

const createEditForm = (shipment: Shipment): EditShipmentForm => ({
  referenceNumber: shipment.referenceNumber,
  customerReference: shipment.customerReference ?? "",
  customerId: shipment.customerId ?? "",
  originPort: shipment.originPort,
  destinationPort: shipment.destinationPort,
  weightKg: shipment.weightKg ? String(shipment.weightKg) : "",
  volumeCbm: shipment.volumeCbm ? String(shipment.volumeCbm) : "",
  estimatedDepartureUtc: toInputDateTime(shipment.estimatedDepartureUtc),
  estimatedArrivalUtc: toInputDateTime(shipment.estimatedArrivalUtc),
  currentLocation: shipment.currentLocation ?? "",
  notes: shipment.notes ?? "",
})

export default function ShipmentsPage() {
  const { toast } = useToast()
  const [shipments, setShipments] = useState<Shipment[]>([])
  const [history, setHistory] = useState<ShipmentStatusHistory[]>([])
  const [historyDialogOpen, setHistoryDialogOpen] = useState(false)
  const [selectedShipment, setSelectedShipment] = useState<Shipment | null>(null)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const [page, setPage] = useState(1)
  const [pageSize] = useState(10)
  const [totalCount, setTotalCount] = useState(0)
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("all")
  const [searchQuery, setSearchQuery] = useState("")
  const [historyLoading, setHistoryLoading] = useState(false)
  const [deleting, setDeleting] = useState(false)

  const [editDialogOpen, setEditDialogOpen] = useState(false)
  const [editForm, setEditForm] = useState<EditShipmentForm | null>(null)
  const [editSubmitting, setEditSubmitting] = useState(false)
  const [editError, setEditError] = useState("")

  const [statusDialogOpen, setStatusDialogOpen] = useState(false)
  const [statusForm, setStatusForm] = useState<StatusUpdateForm>(() => createStatusForm())
  const [statusSubmitting, setStatusSubmitting] = useState(false)
  const [statusError, setStatusError] = useState("")

  const totalPages = useMemo(() => Math.max(1, Math.ceil(totalCount / pageSize)), [totalCount, pageSize])

  const loadShipments = async (pageOverride?: number, statusOverride?: StatusFilter) => {
    setLoading(true)
    setError("")

    const pageNumber = pageOverride ?? page
    const statusValue = statusOverride ?? statusFilter

    try {
      const response = await listShipments({
        page: pageNumber,
        pageSize,
        search: searchQuery.trim() || undefined,
        status: statusValue === "all" ? undefined : statusKeyMap[statusValue],
      })

      setShipments(response.items)
      setTotalCount(response.totalCount)
    } catch (err) {
      setShipments([])
      if (err instanceof ApiError) {
        setError(err.message || "Failed to load shipments. Please try again.")
      } else {
        setError("Unexpected error while loading shipments.")
      }
    } finally {
      setLoading(false)
    }
  }

  const refreshHistory = async (shipment: Shipment) => {
    setHistoryDialogOpen(true)
    setHistoryLoading(true)
    setSelectedShipment(shipment)

    try {
      const result = await getShipmentHistory(shipment.id)
      setHistory(result)
    } catch (err) {
      setHistory([])
      if (err instanceof ApiError) {
        toast({ variant: "destructive", description: err.message || "Unable to load shipment history." })
      } else {
        toast({ variant: "destructive", description: "Unexpected error while loading shipment history." })
      }
    } finally {
      setHistoryLoading(false)
    }
  }

  useEffect(() => {
    loadShipments()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, statusFilter])

  const handleSearch = async () => {
    setPage(1)
    await loadShipments(1)
  }

  const confirmDelete = (shipment: Shipment) => {
    setSelectedShipment(shipment)
    setDeleteDialogOpen(true)
  }

  const handleDelete = async () => {
    if (!selectedShipment) {
      return
    }

    setDeleting(true)
    try {
      await deleteShipment(selectedShipment.id)
      toast({ description: "Shipment deleted successfully." })
      setDeleteDialogOpen(false)
      setSelectedShipment(null)
      await loadShipments(page)
    } catch (err) {
      if (err instanceof ApiError) {
        toast({ variant: "destructive", description: err.message || "Failed to delete shipment." })
      } else {
        toast({ variant: "destructive", description: "Unexpected error while deleting shipment." })
      }
    } finally {
      setDeleting(false)
    }
  }

  const openEdit = (shipment: Shipment) => {
    setSelectedShipment(shipment)
    setEditForm(createEditForm(shipment))
    setEditError("")
    setEditDialogOpen(true)
  }

  const handleEditChange = (field: keyof EditShipmentForm, value: string) => {
    setEditForm((prev) => {
      if (!prev) {
        return prev
      }

      return { ...prev, [field]: value }
    })
  }

  const submitEdit = async (event: React.FormEvent) => {
    event.preventDefault()

    if (!selectedShipment || !editForm) {
      return
    }

    if (!editForm.originPort.trim() || !editForm.destinationPort.trim()) {
      setEditError("Origin and destination ports are required.")
      return
    }

    const weight = Number.parseFloat(editForm.weightKg)
    if (!Number.isFinite(weight) || weight <= 0) {
      setEditError("Weight must be a positive number.")
      return
    }

    setEditSubmitting(true)
    setEditError("")

    try {
      await updateShipment({
        shipmentId: selectedShipment.id,
        customerReference: editForm.customerReference.trim() || undefined,
        customerId: editForm.customerId.trim() || undefined,
        originPort: editForm.originPort.trim(),
        destinationPort: editForm.destinationPort.trim(),
        weightKg: weight,
        volumeCbm: editForm.volumeCbm ? Number.parseFloat(editForm.volumeCbm) : 0,
        estimatedDepartureUtc: toUtcIsoString(editForm.estimatedDepartureUtc) ?? undefined,
        estimatedArrivalUtc: toUtcIsoString(editForm.estimatedArrivalUtc) ?? undefined,
        currentLocation: editForm.currentLocation.trim() || undefined,
        notes: editForm.notes.trim() || undefined,
      })

      toast({ description: "Shipment updated." })
      setEditDialogOpen(false)
      setEditForm(null)
      await loadShipments(page)
      if (historyDialogOpen && selectedShipment) {
        await refreshHistory(selectedShipment)
      }
    } catch (err) {
      if (err instanceof ApiError) {
        setEditError(err.message || "Failed to update shipment.")
      } else {
        setEditError("Unexpected error while updating shipment.")
      }
    } finally {
      setEditSubmitting(false)
    }
  }

  const openStatusDialog = (shipment: Shipment) => {
    setSelectedShipment(shipment)
    setStatusForm(createStatusForm(shipment))
    setStatusError("")
    setStatusDialogOpen(true)
  }

  const submitStatusUpdate = async (event: React.FormEvent) => {
    event.preventDefault()

    if (!selectedShipment) {
      return
    }

    if (!statusForm.eventTimeLocal) {
      setStatusError("Event time is required.")
      return
    }

    setStatusSubmitting(true)
    setStatusError("")

    try {
      await appendShipmentStatus(selectedShipment.id, {
        status: statusForm.status,
        description: statusForm.description.trim() || undefined,
        location: statusForm.location.trim() || undefined,
        eventTimeUtc: toUtcIsoString(statusForm.eventTimeLocal) ?? new Date().toISOString(),
      })

      toast({ description: "Shipment status updated." })
      setStatusDialogOpen(false)
      await loadShipments(page)
      if (historyDialogOpen && selectedShipment) {
        await refreshHistory(selectedShipment)
      }
    } catch (err) {
      if (err instanceof ApiError) {
        setStatusError(err.message || "Failed to append status.")
      } else {
        setStatusError("Unexpected error while appending status.")
      }
    } finally {
      setStatusSubmitting(false)
    }
  }

  const insights = useMemo(() => {
    const totalWeight = shipments.reduce((acc, item) => acc + item.weightKg, 0)
    const inTransit = shipments.filter((item) =>
      [ShipmentStatus.OnVessel, ShipmentStatus.OutForDelivery].includes(item.currentStatus),
    ).length
    const delivered = shipments.filter((item) => item.currentStatus === ShipmentStatus.Delivered).length

    return [
      {
        title: "Listed Shipments",
        value: shipments.length.toString(),
        helper: "Matching current filters",
        icon: Package,
      },
      {
        title: "In Transit",
        value: inTransit.toString(),
        helper: "Vessel or delivery",
        icon: Truck,
      },
      {
        title: "Delivered",
        value: delivered.toString(),
        helper: "Delivered from list",
        icon: CheckCircle2,
      },
      {
        title: "Total Weight",
        value: `${formatNumber(totalWeight)} kg`,
        helper: "Sum of listed weights",
        icon: TrendingUp,
      },
    ]
  }, [shipments])

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Shipments</h1>
          <p className="text-muted-foreground mt-1">Monitor and manage active shipments.</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => loadShipments(page)} disabled={loading}>
            <RefreshCw className={`h-4 w-4 mr-2 ${loading ? "animate-spin" : ""}`} />
            Refresh
          </Button>
          <Link href="/admin/shipments/create">
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              New Shipment
            </Button>
          </Link>
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {insights.map((stat) => {
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
                <div className="text-2xl font-bold text-foreground">{stat.value}</div>
                <p className="text-xs text-muted-foreground mt-1">{stat.helper}</p>
              </CardContent>
            </Card>
          )
        })}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Filters</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-3">
          <div className="col-span-2 flex items-center gap-2">
            <Input
              placeholder="Search by tracking number, reference, customer ref, or location"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
            />
            <Button variant="secondary" onClick={handleSearch} disabled={loading}>
              Search
            </Button>
          </div>
          <Select
            value={statusFilter}
            onValueChange={(value) => {
              const nextStatus = value as StatusFilter
              setStatusFilter(nextStatus)
              setPage(1)
              loadShipments(1, nextStatus)
            }}
          >
            <SelectTrigger>
              <SelectValue placeholder="Filter by status" />
            </SelectTrigger>
            <SelectContent>
              {statusOptions.map((option) => (
                <SelectItem key={option.value} value={option.value}>
                  {option.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </CardContent>
      </Card>

      {error && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      <Card>
        <CardHeader>
          <CardTitle>Shipment List</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Tracking #</TableHead>
                  <TableHead>Reference</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Origin</TableHead>
                  <TableHead>Destination</TableHead>
                  <TableHead>Weight (kg)</TableHead>
                  <TableHead>ETA</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {loading ? (
                  <TableRow>
                    <TableCell colSpan={8} className="text-center py-6">
                      <div className="inline-flex items-center gap-2 text-sm text-muted-foreground">
                        <Loader2 className="h-4 w-4 animate-spin" />
                        Loading shipments...
                      </div>
                    </TableCell>
                  </TableRow>
                ) : shipments.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={8} className="text-center py-6 text-sm text-muted-foreground">
                      No shipments found for the current filters.
                    </TableCell>
                  </TableRow>
                ) : (
                  shipments.map((shipment) => (
                    <TableRow key={shipment.id}>
                      <TableCell className="font-medium">{shipment.trackingNumber}</TableCell>
                      <TableCell>
                        <div className="flex flex-col">
                          <span className="font-medium text-foreground">{shipment.referenceNumber}</span>
                          {shipment.customerReference && (
                            <span className="text-xs text-muted-foreground">
                              Customer ref: {shipment.customerReference}
                            </span>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge variant="secondary">{statusLabel(shipment.currentStatus)}</Badge>
                      </TableCell>
                      <TableCell>{shipment.originPort}</TableCell>
                      <TableCell>{shipment.destinationPort}</TableCell>
                      <TableCell>{formatNumber(shipment.weightKg)}</TableCell>
                      <TableCell>{formatDate(shipment.estimatedArrivalUtc)}</TableCell>
                      <TableCell className="text-right">
                        <div className="flex justify-end gap-2">
                          <Button
                            variant="outline"
                            size="icon"
                            onClick={() => openStatusDialog(shipment)}
                            title="Append status"
                          >
                            <PlusCircle className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="outline"
                            size="icon"
                            onClick={() => openEdit(shipment)}
                            title="Edit shipment"
                          >
                            <Pencil className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="outline"
                            size="icon"
                            onClick={() => refreshHistory(shipment)}
                            title="View history"
                          >
                            <History className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="destructive"
                            size="icon"
                            onClick={() => confirmDelete(shipment)}
                            title="Delete shipment"
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>

          <div className="flex items-center justify-between text-sm text-muted-foreground">
            <span>
              Showing {(page - 1) * pageSize + 1}-{Math.min(page * pageSize, totalCount)} of {totalCount} shipments
            </span>
            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm" onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page === 1}>
                Previous
              </Button>
              <span>
                Page {page} of {totalPages}
              </span>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
              >
                Next
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      <Dialog open={historyDialogOpen} onOpenChange={setHistoryDialogOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Shipment History</DialogTitle>
            <DialogDescription>
              {selectedShipment ? `Tracking #${selectedShipment.trackingNumber}` : "Shipment timeline"}
            </DialogDescription>
          </DialogHeader>
          {historyLoading ? (
            <div className="flex items-center justify-center py-6 text-muted-foreground gap-2">
              <Loader2 className="h-4 w-4 animate-spin" />
              Loading history...
            </div>
          ) : history.length === 0 ? (
            <p className="text-sm text-muted-foreground">No history entries available for this shipment.</p>
          ) : (
            <div className="space-y-3 max-h-96 overflow-y-auto pr-2">
              {history.map((item) => (
                <div key={item.id} className="border border-border rounded-lg p-4 space-y-1">
                  <div className="flex items-center gap-2">
                    <Badge variant="secondary">{statusLabel(item.status)}</Badge>
                    <span className="text-xs text-muted-foreground">{formatDateTime(item.eventTimeUtc)}</span>
                  </div>
                  {item.location && (
                    <p className="text-sm text-muted-foreground flex items-center gap-2">
                      <MapPin className="h-4 w-4" /> {item.location}
                    </p>
                  )}
                  {item.description && <p className="text-sm text-foreground">{item.description}</p>}
                </div>
              ))}
            </div>
          )}
        </DialogContent>
      </Dialog>

      <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Edit shipment</DialogTitle>
            <DialogDescription>Update the core shipment attributes.</DialogDescription>
          </DialogHeader>
          {editForm && (
            <form className="space-y-4" onSubmit={submitEdit}>
              <div className="grid gap-4 md:grid-cols-2">
                <div className="space-y-2">
                  <label htmlFor="editReference" className="text-sm font-medium text-muted-foreground">
                    Reference Number
                  </label>
                  <Input
                    id="editReference"
                    value={editForm.referenceNumber}
                    readOnly
                    className="cursor-text bg-muted/50 focus-visible:ring-0"
                  />
                  <p className="text-xs text-muted-foreground">Generated automatically and cannot be changed.</p>
                </div>
                <div className="space-y-2">
                  <label htmlFor="editCustomer" className="text-sm font-medium text-muted-foreground">
                    Customer ID
                  </label>
                  <Input
                    id="editCustomer"
                    value={editForm.customerId}
                    onChange={(e) => handleEditChange("customerId", e.target.value)}
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label htmlFor="editCustomerReference" className="text-sm font-medium text-muted-foreground">
                  Customer Reference
                </label>
                <Input
                  id="editCustomerReference"
                  value={editForm.customerReference}
                  onChange={(e) => handleEditChange("customerReference", e.target.value)}
                  placeholder="Optional external reference"
                />
              </div>

              <div className="grid gap-4 md:grid-cols-2">
                <div className="space-y-2">
                  <label htmlFor="editOrigin" className="text-sm font-medium text-muted-foreground">
                    Origin Port
                  </label>
                  <Input
                    id="editOrigin"
                    value={editForm.originPort}
                    onChange={(e) => handleEditChange("originPort", e.target.value)}
                    required
                  />
                </div>
                <div className="space-y-2">
                  <label htmlFor="editDestination" className="text-sm font-medium text-muted-foreground">
                    Destination Port
                  </label>
                  <Input
                    id="editDestination"
                    value={editForm.destinationPort}
                    onChange={(e) => handleEditChange("destinationPort", e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="grid gap-4 md:grid-cols-2">
                <div className="space-y-2">
                  <label htmlFor="editWeight" className="text-sm font-medium text-muted-foreground">
                    Weight (kg)
                  </label>
                  <Input
                    id="editWeight"
                    type="number"
                    min="0"
                    step="0.01"
                    value={editForm.weightKg}
                    onChange={(e) => handleEditChange("weightKg", e.target.value)}
                    required
                  />
                </div>
                <div className="space-y-2">
                  <label htmlFor="editVolume" className="text-sm font-medium text-muted-foreground">
                    Volume (cbm)
                  </label>
                  <Input
                    id="editVolume"
                    type="number"
                    min="0"
                    step="0.01"
                    value={editForm.volumeCbm}
                    onChange={(e) => handleEditChange("volumeCbm", e.target.value)}
                  />
                </div>
              </div>

              <div className="grid gap-4 md:grid-cols-2">
                <div className="space-y-2">
                  <label htmlFor="editDeparture" className="text-sm font-medium text-muted-foreground">
                    Estimated Departure (local)
                  </label>
                  <Input
                    id="editDeparture"
                    type="datetime-local"
                    value={editForm.estimatedDepartureUtc}
                    onChange={(e) => handleEditChange("estimatedDepartureUtc", e.target.value)}
                  />
                </div>
                <div className="space-y-2">
                  <label htmlFor="editArrival" className="text-sm font-medium text-muted-foreground">
                    Estimated Arrival (local)
                  </label>
                  <Input
                    id="editArrival"
                    type="datetime-local"
                    value={editForm.estimatedArrivalUtc}
                    onChange={(e) => handleEditChange("estimatedArrivalUtc", e.target.value)}
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label htmlFor="editLocation" className="text-sm font-medium text-muted-foreground">
                  Current Location
                </label>
                <Input
                  id="editLocation"
                  value={editForm.currentLocation}
                  onChange={(e) => handleEditChange("currentLocation", e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <label htmlFor="editNotes" className="text-sm font-medium text-muted-foreground">
                  Notes
                </label>
                <Textarea
                  id="editNotes"
                  rows={4}
                  value={editForm.notes}
                  onChange={(e) => handleEditChange("notes", e.target.value)}
                />
              </div>

              {editError && (
                <Alert variant="destructive">
                  <AlertCircle className="h-4 w-4" />
                  <AlertDescription>{editError}</AlertDescription>
                </Alert>
              )}

              <div className="flex justify-end gap-2">
                <Button type="button" variant="outline" onClick={() => setEditDialogOpen(false)} disabled={editSubmitting}>
                  Cancel
                </Button>
                <Button type="submit" disabled={editSubmitting}>
                  {editSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : "Save changes"}
                </Button>
              </div>
            </form>
          )}
        </DialogContent>
      </Dialog>

      <Dialog open={statusDialogOpen} onOpenChange={setStatusDialogOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Append status</DialogTitle>
            <DialogDescription>Add a new timeline entry for the shipment.</DialogDescription>
          </DialogHeader>
          <form className="space-y-4" onSubmit={submitStatusUpdate}>
            <div className="space-y-2">
              <label className="text-sm font-medium text-muted-foreground" htmlFor="status-select">
                Status
              </label>
              <Select
                value={statusForm.status.toString()}
                onValueChange={(value) => setStatusForm((prev) => ({ ...prev, status: Number(value) as ShipmentStatus }))}
              >
                <SelectTrigger id="status-select">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {statusOptions
                    .filter((option) => option.value !== "all")
                    .map((option) => (
                      <SelectItem key={option.value} value={statusKeyMap[option.value].toString()}>
                        {option.label}
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-muted-foreground" htmlFor="status-location">
                Location
              </label>
              <Input
                id="status-location"
                value={statusForm.location}
                onChange={(e) => setStatusForm((prev) => ({ ...prev, location: e.target.value }))}
                placeholder="Warehouse 12"
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-muted-foreground" htmlFor="status-time">
                Event time (local)
              </label>
              <Input
                id="status-time"
                type="datetime-local"
                value={statusForm.eventTimeLocal}
                onChange={(e) => setStatusForm((prev) => ({ ...prev, eventTimeLocal: e.target.value }))}
                required
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-muted-foreground" htmlFor="status-description">
                Description
              </label>
              <Textarea
                id="status-description"
                value={statusForm.description}
                onChange={(e) => setStatusForm((prev) => ({ ...prev, description: e.target.value }))}
                rows={3}
                placeholder="Describe the event or update"
              />
            </div>

            {statusError && (
              <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>{statusError}</AlertDescription>
              </Alert>
            )}

            <div className="flex justify-end gap-2">
              <Button type="button" variant="outline" onClick={() => setStatusDialogOpen(false)} disabled={statusSubmitting}>
                Cancel
              </Button>
              <Button type="submit" disabled={statusSubmitting}>
                {statusSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : "Append status"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete shipment</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. Are you sure you want to delete{" "}
              <span className="font-medium text-foreground">{selectedShipment?.trackingNumber}</span>?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleting}>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={handleDelete} disabled={deleting}>
              {deleting ? <Loader2 className="h-4 w-4 animate-spin" /> : "Delete"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
