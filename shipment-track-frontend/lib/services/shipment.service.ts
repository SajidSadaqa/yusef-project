import { apiFetch, ApiError } from "@/lib/api/http"
import { getValidAccessToken, refreshTokens } from "@/lib/services/auth.service"
import type { PagedResult } from "@/lib/types/pagination"
import type {
  AppendShipmentStatusPayload,
  CreateShipmentPayload,
  Shipment,
  ShipmentStatus,
  ShipmentStatusHistory,
  UpdateShipmentPayload,
  PublicTracking,
} from "@/lib/types/shipment"

const SHIPMENT_STATUS_NAMES = [
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

type ShipmentStatusName = (typeof SHIPMENT_STATUS_NAMES)[number]

type ListShipmentsParams = {
  page?: number
  pageSize?: number
  status?: ShipmentStatus | ShipmentStatusName
  fromDateUtc?: string
  toDateUtc?: string
  search?: string
}

const withAuthorization = async <TResponse>(path: string, init: RequestInit = {}) => {
  const baseInit: RequestInit = { ...init }
  let token = await getValidAccessToken()

  if (!token) {
    throw new ApiError(401, "You must be signed in to perform this action.", null)
  }

  const headers = new Headers(baseInit.headers ?? undefined)
  headers.set("Authorization", `Bearer ${token}`)
  baseInit.headers = headers

  try {
    return await apiFetch<TResponse>(path, baseInit)
  } catch (error) {
    if (error instanceof ApiError && error.status === 401) {
      const refreshed = await refreshTokens()
      token = refreshed?.accessToken

      if (!token) {
        throw new ApiError(401, "Session expired. Please sign in again.", error.payload)
      }

      headers.set("Authorization", `Bearer ${token}`)
      return await apiFetch<TResponse>(path, baseInit)
    }

    throw error
  }
}

export const listShipments = async (params: ListShipmentsParams = {}) => {
  const query = new URLSearchParams()

  if (params.page) {
    query.set("pageNumber", params.page.toString())
  }

  if (params.pageSize) {
    query.set("pageSize", params.pageSize.toString())
  }

  if (params.status !== undefined) {
    if (typeof params.status === "number") {
      const name = SHIPMENT_STATUS_NAMES[params.status]
      if (name) {
        query.set("status", name)
      }
    } else {
      query.set("status", params.status)
    }
  }

  if (params.fromDateUtc) {
    query.set("fromDateUtc", params.fromDateUtc)
  }

  if (params.toDateUtc) {
    query.set("toDateUtc", params.toDateUtc)
  }

  if (params.search) {
    query.set("search", params.search)
  }

  const suffix = query.toString() ? `?${query.toString()}` : ""
  return withAuthorization<PagedResult<Shipment>>(`/shipments${suffix}`)
}

export const getShipmentHistory = async (shipmentId: string) => {
  return withAuthorization<ShipmentStatusHistory[]>(`/shipments/${shipmentId}/history`)
}

export const createShipment = async (payload: CreateShipmentPayload) => {
  return withAuthorization<Shipment>("/shipments", {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export const updateShipment = async (payload: UpdateShipmentPayload) => {
  return withAuthorization<Shipment>(`/shipments/${payload.shipmentId}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  })
}

export const appendShipmentStatus = async (shipmentId: string, payload: AppendShipmentStatusPayload) => {
  return withAuthorization<Shipment>(`/shipments/${shipmentId}/status`, {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export const deleteShipment = async (shipmentId: string) => {
  return withAuthorization<void>(`/shipments/${shipmentId}`, {
    method: "DELETE",
  })
}

export const trackShipmentPublic = async (trackingNumber: string) => {
  return apiFetch<PublicTracking>(`/public/track/${encodeURIComponent(trackingNumber)}`)
}
