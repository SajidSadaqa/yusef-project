export enum ShipmentStatus {
  Received = 0,
  Packed = 1,
  AtOriginPort = 2,
  OnVessel = 3,
  ArrivedToPort = 4,
  CustomsCleared = 5,
  OutForDelivery = 6,
  Delivered = 7,
  Returned = 8,
  Cancelled = 9,
}

export type ShipmentStatusHistory = {
  id: string
  status: ShipmentStatus
  description?: string | null
  location?: string | null
  eventTimeUtc: string
}

export type Shipment = {
  id: string
  trackingNumber: string
  referenceNumber: string
  customerId?: string | null
  originPort: string
  destinationPort: string
  weightKg: number
  volumeCbm: number
  currentStatus: ShipmentStatus
  estimatedDepartureUtc?: string | null
  estimatedArrivalUtc?: string | null
  currentLocation?: string | null
  notes?: string | null
  createdAtUtc: string
  updatedAtUtc?: string | null
  statusHistory: ShipmentStatusHistory[]
}

export type PublicTracking = {
  trackingNumber: string
  currentStatus: ShipmentStatus
  currentLocation?: string | null
  estimatedArrivalUtc?: string | null
  statusHistory: ShipmentStatusHistory[]
}

export type AppendShipmentStatusPayload = {
  status: ShipmentStatus
  description?: string
  location?: string
  eventTimeUtc: string
}

export type CreateShipmentPayload = {
  referenceNumber: string
  customerId?: string | null
  originPort: string
  destinationPort: string
  weightKg: number
  volumeCbm: number
  estimatedDepartureUtc?: string | null
  estimatedArrivalUtc?: string | null
  currentLocation?: string | null
  notes?: string | null
}

export type UpdateShipmentPayload = CreateShipmentPayload & {
  shipmentId: string
}
