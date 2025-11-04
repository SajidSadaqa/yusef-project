import { getInternalApiBaseUrl, getPublicApiBaseUrl } from "@/lib/config"

export class ApiError extends Error {
  status: number
  payload: unknown

  constructor(status: number, message: string, payload: unknown) {
    super(message)
    this.status = status
    this.payload = payload
  }
}

const resolveBaseUrl = () => {
  if (typeof window === "undefined") {
    return getInternalApiBaseUrl()
  }

  return getPublicApiBaseUrl()
}

const buildUrl = (path: string) => {
  if (/^https?:\/\//i.test(path)) {
    return path
  }

  const base = resolveBaseUrl().replace(/\/$/, "")
  const normalizedPath = path.startsWith("/") ? path : `/${path}`
  return `${base}${normalizedPath}`
}

const shouldIncludeJsonContentType = (init?: RequestInit) => {
  if (!init?.body) {
    return false
  }

  if (init.body instanceof FormData || init.body instanceof Blob) {
    return false
  }

  return true
}

export const apiFetch = async <TResponse>(
  path: string,
  init: RequestInit = {},
  options: { parseJson?: boolean } = {},
): Promise<TResponse> => {
  const url = buildUrl(path)
  const headers = new Headers(init.headers ?? undefined)

  if (shouldIncludeJsonContentType(init) && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json")
  }

  if (!headers.has("Accept")) {
    headers.set("Accept", "application/json")
  }

  const response = await fetch(url, {
    ...init,
    headers,
  })

  if (response.status === 204 || response.status === 205) {
    return undefined as TResponse
  }

  const responseText = await response.text()
  const parseJson = options.parseJson ?? true
  let parsedBody: unknown = responseText

  if (parseJson && responseText) {
    try {
      parsedBody = JSON.parse(responseText)
    } catch {
      // Keep original text payload when JSON parsing fails
    }
  }

  if (!response.ok) {
    const message =
      typeof parsedBody === "object" && parsedBody !== null && "title" in parsedBody
        ? String((parsedBody as { title?: unknown }).title)
        : response.statusText || "API request failed"

    throw new ApiError(response.status, message, parsedBody)
  }

  return parsedBody as TResponse
}
