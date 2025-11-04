export type TokenResponse = {
  accessToken: string
  refreshToken: string
  expiresAtUtc: string
  claims: Record<string, unknown>
}

export type LoginPayload = {
  email: string
  password: string
}

export type RegisterPayload = {
  email: string
  password: string
  firstName?: string
  lastName?: string
  phoneNumber?: string
}
