using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorLight;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Infrastructure.Options;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Sends transactional emails via SMTP (e.g. MailHog) using Razor templates.
/// </summary>
public sealed class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly RazorLightEngine _razorEngine;

    public SmtpEmailService(
        IOptions<SmtpOptions> options,
        IHostEnvironment hostEnvironment,
        ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            throw new InvalidOperationException("SMTP host is not configured.");
        }

        var templatesPath = Path.Combine(AppContext.BaseDirectory, "Email", "Templates");
        _razorEngine = new RazorLightEngineBuilder()
            .UseFileSystemProject(templatesPath)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task SendTemplateEmailAsync(
        string to,
        string subject,
        string templateName,
        IReadOnlyDictionary<string, object?> model,
        CancellationToken cancellationToken = default)
    {
        dynamic viewModel = new ExpandoObject();
        var expando = (IDictionary<string, object?>)viewModel;
        foreach (var kvp in model)
        {
            expando[kvp.Key] = kvp.Value;
        }

        var htmlBody = await _razorEngine.CompileRenderAsync($"{templateName}.cshtml", viewModel);

        using var message = new MailMessage
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8
        };

        var fromAddress = DetermineFromAddress();
        message.From = fromAddress;
        message.To.Add(to);

        using var smtpClient = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.UseSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        if (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password))
        {
            smtpClient.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        try
        {
            await smtpClient.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMTP email to {Recipient} using host {Host}:{Port}", to, _options.Host, _options.Port);
            throw;
        }
    }

    private MailAddress DetermineFromAddress()
    {
        if (!string.IsNullOrWhiteSpace(_options.FromEmail))
        {
            return string.IsNullOrWhiteSpace(_options.FromName)
                ? new MailAddress(_options.FromEmail!)
                : new MailAddress(_options.FromEmail!, _options.FromName);
        }

        // Fallback to RFC 2606 test domain to avoid invalid sender.
        return new MailAddress("noreply@shipmenttracking.test", "Shipment Tracking (Test)");
    }
}
