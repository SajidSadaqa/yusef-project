const DEFAULT_PUBLIC_API_URL = "http://localhost:8080/api"
const DEFAULT_INTERNAL_API_URL = "http://localhost:8080/api"

const normalizeBaseUrl = (value?: string) => {
  if (!value) {
    return undefined
  }

  return value.endsWith("/") ? value.slice(0, -1) : value
}

export const getPublicApiBaseUrl = () => {
  return normalizeBaseUrl(process.env.NEXT_PUBLIC_API_URL) ?? DEFAULT_PUBLIC_API_URL
}

export const getInternalApiBaseUrl = () => {
  if (typeof process === "undefined") {
    return getPublicApiBaseUrl()
  }

  return normalizeBaseUrl(process.env.API_INTERNAL_URL) ?? getPublicApiBaseUrl() ?? DEFAULT_INTERNAL_API_URL
}
