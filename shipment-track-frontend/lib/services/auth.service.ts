import { apiFetch, ApiError } from "@/lib/api/http"
import type { LoginPayload, RegisterPayload, TokenResponse } from "@/lib/types/auth"

const SESSION_STORAGE_KEY = "shipment-track.session"

type SessionState = TokenResponse

const getStorage = () => {
  if (typeof window === "undefined") {
    return null
  }

  return window.localStorage
}

const readSession = (): SessionState | null => {
  const storage = getStorage()
  if (!storage) {
    return null
  }

  const raw = storage.getItem(SESSION_STORAGE_KEY)
  if (!raw) {
    return null
  }

  try {
    return JSON.parse(raw) as SessionState
  } catch {
    storage.removeItem(SESSION_STORAGE_KEY)
    return null
  }
}

const writeSession = (session: SessionState | null) => {
  const storage = getStorage()
  if (!storage) {
    return
  }

  if (!session) {
    storage.removeItem(SESSION_STORAGE_KEY)
    return
  }

  storage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session))
}

const isExpired = (expiresAtUtc: string, leewaySeconds = 60) => {
  const deadline = new Date(expiresAtUtc).getTime()
  const now = Date.now()
  return deadline <= now + leewaySeconds * 1000
}

let refreshPromise: Promise<SessionState | null> | null = null

const setActiveSession = (session: SessionState | null) => {
  writeSession(session)
  // Dispatch custom event to notify components of auth state change
  if (typeof window !== "undefined") {
    const event = new Event(session ? "login" : "logout")
    window.dispatchEvent(event)
  }
}

export const getSession = (): SessionState | null => readSession()

export const isAuthenticated = () => {
  const session = readSession()
  return Boolean(session?.accessToken && !isExpired(session.expiresAtUtc))
}

export const clearSession = () => {
  setActiveSession(null)
}

export const login = async (payload: LoginPayload) => {
  const response = await apiFetch<TokenResponse>("/auth/login", {
    method: "POST",
    body: JSON.stringify(payload),
  })

  setActiveSession(response)
  return response
}

export const register = async (payload: RegisterPayload & { verificationCallbackUrl?: string; dashboardUrl?: string }) => {
  const response = await apiFetch<object>("/auth/register", {
    method: "POST",
    body: JSON.stringify({
      email: payload.email,
      password: payload.password,
      firstName: payload.firstName ?? null,
      lastName: payload.lastName ?? null,
      phoneNumber: payload.phoneNumber ?? null,
      verificationCallbackUrl: payload.verificationCallbackUrl ?? null,
      dashboardUrl: payload.dashboardUrl ?? null,
    }),
  })

  return response
}

export const refreshTokens = async () => {
  const session = readSession()

  if (!session?.refreshToken) {
    setActiveSession(null)
    return null
  }

  if (!refreshPromise) {
    refreshPromise = apiFetch<TokenResponse>("/auth/refresh", {
      method: "POST",
      body: JSON.stringify({ refreshToken: session.refreshToken }),
    })
      .then((newSession) => {
        setActiveSession(newSession)
        return newSession
      })
      .catch((error) => {
        setActiveSession(null)
        if (error instanceof ApiError && error.status === 401) {
          return null
        }

        throw error
      })
      .finally(() => {
        refreshPromise = null
      })
  }

  return refreshPromise
}

export const getValidAccessToken = async (): Promise<string | null> => {
  const session = readSession()

  if (!session) {
    return null
  }

  if (!isExpired(session.expiresAtUtc)) {
    return session.accessToken
  }

  const refreshed = await refreshTokens()
  return refreshed?.accessToken ?? null
}

export const logout = () => {
  setActiveSession(null)
}

export const sendPasswordReset = async (email: string, resetCallbackUrl?: string) => {
  return apiFetch("/auth/forgot-password", {
    method: "POST",
    body: JSON.stringify({
      email,
      resetCallbackUrl: resetCallbackUrl ?? null,
    }),
  })
}

export const resetPassword = async (payload: { userId: string; token: string; newPassword: string }) => {
  return apiFetch("/auth/reset-password", {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export const verifyEmail = async (payload: { userId: string; token: string }) => {
  return apiFetch("/auth/verify-email", {
    method: "POST",
    body: JSON.stringify(payload),
  })
}
