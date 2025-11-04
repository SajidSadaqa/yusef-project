using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Integration.Tests.Infrastructure;

/// <summary>
/// Test double that captures outgoing email requests without performing network calls.
/// </summary>
public sealed class FakeEmailService : IEmailService
{
    private readonly List<SentEmail> _emails = new();

    /// <summary>
    /// Gets the immutable collection of emails that were captured during a test run.
    /// </summary>
    public IReadOnlyCollection<SentEmail> Emails => _emails.AsReadOnly();

    /// <inheritdoc/>
    public Task SendTemplateEmailAsync(
        string to,
        string subject,
        string templateName,
        IReadOnlyDictionary<string, object?> model,
        CancellationToken cancellationToken = default)
    {
        _emails.Add(new SentEmail(to, subject, templateName, model));
        return Task.CompletedTask;
    }

    public sealed record SentEmail(
        string To,
        string Subject,
        string TemplateName,
        IReadOnlyDictionary<string, object?> Model);
}
