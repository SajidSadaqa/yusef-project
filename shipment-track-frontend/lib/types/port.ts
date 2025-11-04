export type Port = {
  id: string
  code: string
  name: string
  country: string
  city?: string | null
  isActive: boolean
  createdAtUtc: string
  updatedAtUtc?: string | null
}

export type CreatePortPayload = {
  name: string
  country: string
  city?: string | null
  code?: string | null // Optional: admin can override auto-generated code
}

export type UpdatePortPayload = {
  name: string
  country: string
  city?: string | null
  isActive: boolean
}

export type PortFilters = {
  activeOnly?: boolean
  searchTerm?: string
  country?: string
}
