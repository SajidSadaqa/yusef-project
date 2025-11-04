namespace ShipmentTracking.WebApi.Contracts.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? VerificationCallbackUrl,
    string? DashboardUrl);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string RefreshToken, string? IpAddress);

public sealed record VerifyEmailRequest(string UserId, string Token);

public sealed record ForgotPasswordRequest(string Email, string? ResetCallbackUrl);

public sealed record ResetPasswordRequest(string UserId, string Token, string NewPassword);

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAtUtc,
    IDictionary<string, object> Claims);
