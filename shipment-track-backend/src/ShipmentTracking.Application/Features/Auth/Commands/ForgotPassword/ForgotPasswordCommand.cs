using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// Command to initiate a password reset workflow.
/// </summary>
public sealed record ForgotPasswordCommand(string Email, string? ResetCallbackUrl) : IRequest;

/// <summary>
/// Handles <see cref="ForgotPasswordCommand"/>.
/// </summary>
public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            // Do not reveal that the email does not exist.
            return Unit.Value;
        }

        var tokenResult = await _identityService.GeneratePasswordResetTokenAsync(user.Id, cancellationToken);

        if (!tokenResult.Succeeded || tokenResult.Value is null)
        {
            return Unit.Value;
        }

        var resetUrl = BuildResetUrl(request.ResetCallbackUrl, user.Id, tokenResult.Value);

        var model = new Dictionary<string, object?>
        {
            ["email"] = request.Email,
            ["firstName"] = user.FirstName,
            ["resetToken"] = tokenResult.Value,
            ["userId"] = user.Id,
            ["resetUrl"] = resetUrl
        };

        _logger.LogInformation("Attempting to send password reset email to {Email}", request.Email);

        await _emailService.SendTemplateEmailAsync(
            request.Email,
            "Reset your Shipment Tracking password",
            "ResetPassword",
            model,
            cancellationToken);

        _logger.LogInformation("Successfully sent password reset email to {Email}", request.Email);

        return Unit.Value;
    }

    private static string? BuildResetUrl(string? baseUrl, string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return null;
        }

        var separator = baseUrl.Contains("?", StringComparison.Ordinal) ? "&" : "?";
        return $"{baseUrl}{separator}userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";
    }
}
