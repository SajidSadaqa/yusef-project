using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShipmentTracking.Application.Common.Exceptions;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command used to register a new user.
/// </summary>
public sealed record RegisterCommand(
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? VerificationCallbackUrl,
    string? DashboardUrl) : IRequest<string>;

/// <summary>
/// Handles <see cref="RegisterCommand"/>.
/// </summary>
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IIdentityService identityService,
        IEmailService emailService,
        ILogger<RegisterCommandHandler> logger)
    {
        _identityService = identityService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var createResult = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            cancellationToken);

        if (!createResult.Succeeded || createResult.Value is null)
        {
            throw new ShipmentTracking.Application.Common.Exceptions.ValidationException(createResult.Errors);
        }

        var userId = createResult.Value;

        await _identityService.EnsureRoleExistsAsync("Customer", cancellationToken);
        await _identityService.AddUserToRoleAsync(userId, "Customer", cancellationToken);

        var verificationTokenResult =
            await _identityService.GenerateEmailConfirmationTokenAsync(userId, cancellationToken);

        if (!verificationTokenResult.Succeeded || verificationTokenResult.Value is null)
        {
            throw new ApplicationException("Failed to generate email verification token.");
        }

        var verificationModel = new Dictionary<string, object?>
        {
            ["email"] = request.Email,
            ["firstName"] = request.FirstName,
            ["verificationToken"] = verificationTokenResult.Value,
            ["userId"] = userId,
            ["verificationUrl"] = BuildVerificationUrl(
                request.VerificationCallbackUrl,
                userId,
                verificationTokenResult.Value),
        };

        try
        {
            await _emailService.SendTemplateEmailAsync(
                request.Email,
                "Verify your Shipment Tracking account",
                "VerifyEmail",
                verificationModel,
                cancellationToken);

            var welcomeModel = new Dictionary<string, object?>
            {
                ["firstName"] = request.FirstName ?? request.Email,
                ["dashboardUrl"] = request.DashboardUrl ?? string.Empty
            };

            await _emailService.SendTemplateEmailAsync(
                request.Email,
                "Welcome to Shipment Tracking",
                "WelcomeEmail",
                welcomeModel,
                cancellationToken);

            _logger.LogInformation("Successfully sent verification and welcome emails to {Email}", request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send verification/welcome emails to {Email}. User was created successfully but emails were not sent.", request.Email);
        }

        return userId;
    }

    private static string? BuildVerificationUrl(string? callbackUrl, string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            return null;
        }

        var separator = callbackUrl.Contains("?", StringComparison.Ordinal) ? "&" : "?";
        return $"{callbackUrl}{separator}userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";
    }
}
