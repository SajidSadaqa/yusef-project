using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShipmentTracking.Application.Common.Interfaces;

/// <summary>
/// Contract for sending transactional emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email using the configured provider.
    /// </summary>
    /// <param name="to">Recipient email address.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="templateName">Razor template name.</param>
    /// <param name="model">Template data model.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendTemplateEmailAsync(
        string to,
        string subject,
        string templateName,
        IReadOnlyDictionary<string, object?> model,
        CancellationToken cancellationToken = default);
}
