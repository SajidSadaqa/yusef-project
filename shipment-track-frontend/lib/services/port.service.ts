import { apiFetch, ApiError } from "@/lib/api/http"
import { getValidAccessToken, refreshTokens } from "@/lib/services/auth.service"
import type { Port, CreatePortPayload, UpdatePortPayload, PortFilters } from "@/lib/types/port"

export const listPorts = async (filters?: PortFilters): Promise<Port[]> => {
  const params = new URLSearchParams()

  if (filters?.activeOnly !== undefined) {
    params.append("activeOnly", String(filters.activeOnly))
  } else {
    params.append("activeOnly", "true")
  }

  if (filters?.searchTerm) {
    params.append("searchTerm", filters.searchTerm)
  }

  if (filters?.country) {
    params.append("country", filters.country)
  }

  const queryString = params.toString()
  return apiFetch<Port[]>(`/ports${queryString ? `?${queryString}` : ""}`)
}

export const getPortById = async (id: string): Promise<Port> => {
  return apiFetch<Port>(`/ports/${id}`)
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

export const createPort = async (payload: CreatePortPayload): Promise<Port> => {
  return withAuthorization<Port>("/ports", {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export const updatePort = async (id: string, payload: UpdatePortPayload): Promise<Port> => {
  return withAuthorization<Port>(`/ports/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  })
}

export const deletePort = async (id: string): Promise<void> => {
  return withAuthorization<void>(`/ports/${id}`, {
    method: "DELETE",
  })
}
