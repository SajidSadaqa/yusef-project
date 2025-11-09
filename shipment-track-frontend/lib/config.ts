// Environment validation
const validateEnvVar = (value: string | undefined, name: string, fallback?: string): string => {
  // In production, require environment variables to be set
  if (process.env.NODE_ENV === 'production' && !value) {
    if (!fallback) {
      throw new Error(`Missing required environment variable: ${name}`)
    }
    console.warn(`Using fallback for ${name}. Set this in production!`)
  }

  return value ?? fallback ?? ''
}

const normalizeBaseUrl = (value?: string) => {
  if (!value) {
    return undefined
  }

  return value.endsWith("/") ? value.slice(0, -1) : value
}

export const getPublicApiBaseUrl = () => {
  const url = validateEnvVar(
    process.env.NEXT_PUBLIC_API_URL,
    'NEXT_PUBLIC_API_URL',
    process.env.NODE_ENV === 'development' ? 'http://localhost:8080/api' : undefined
  )
  return normalizeBaseUrl(url) ?? ''
}

export const getInternalApiBaseUrl = () => {
  if (typeof process === "undefined") {
    return getPublicApiBaseUrl()
  }

  const url = validateEnvVar(
    process.env.API_INTERNAL_URL,
    'API_INTERNAL_URL',
    process.env.NODE_ENV === 'development' ? 'http://localhost:8080/api' : undefined
  )
  return normalizeBaseUrl(url) ?? getPublicApiBaseUrl()
}
