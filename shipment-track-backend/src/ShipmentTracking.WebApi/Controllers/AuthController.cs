using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Application.Features.Auth.Commands.ForgotPassword;
using ShipmentTracking.Application.Features.Auth.Commands.Login;
using ShipmentTracking.Application.Features.Auth.Commands.RefreshToken;
using ShipmentTracking.Application.Features.Auth.Commands.Register;
using ShipmentTracking.Application.Features.Auth.Commands.ResetPassword;
using ShipmentTracking.Application.Features.Auth.Commands.VerifyEmail;
using ShipmentTracking.Application.Features.Auth.Dto;
using ShipmentTracking.WebApi.Contracts.Auth;

namespace ShipmentTracking.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.VerificationCallbackUrl,
            request.DashboardUrl);

        var userId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = userId }, new { userId });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password, HttpContext.Connection.RemoteIpAddress?.ToString());
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(MapTokenResponse(result));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(
            request.RefreshToken,
            request.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString());

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(MapTokenResponse(result));
    }

    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new VerifyEmailCommand(request.UserId, request.Token);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ForgotPasswordCommand(request.Email, request.ResetCallbackUrl);
        await _mediator.Send(command, cancellationToken);
        return Accepted();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request.UserId, request.Token, request.NewPassword);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    private static TokenResponse MapTokenResponse(TokenResponseDto dto) =>
        new(dto.AccessToken, dto.RefreshToken, dto.ExpiresAtUtc, dto.Claims);
}
