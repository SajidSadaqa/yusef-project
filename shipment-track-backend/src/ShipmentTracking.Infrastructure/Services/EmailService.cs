using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorLight;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Infrastructure.Options;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Sends transactional emails using the Resend HTTP API and Razor templates.
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ResendOptions _options;
    private readonly ILogger<EmailService> _logger;
    private readonly RazorLightEngine _razorEngine;

    public EmailService(
        HttpClient httpClient,
        IOptions<ResendOptions> options,
        IHostEnvironment hostEnvironment,
        ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress ??= new Uri("https://api.resend.com/");
        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
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
        dynamic viewModel = new System.Dynamic.ExpandoObject();
        var expando = (IDictionary<string, object?>)viewModel;
        foreach (var kvp in model)
        {
            expando[kvp.Key] = kvp.Value;
        }

        var htmlBody = await _razorEngine.CompileRenderAsync($"{templateName}.cshtml", viewModel);

        var payload = new
        {
            from = string.IsNullOrWhiteSpace(_options.FromName)
                ? _options.FromEmail
                : $"{_options.FromName} <{_options.FromEmail}>",
            to = new[] { to },
            subject,
            html = htmlBody
        };

        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.PostAsync("emails", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to send email via Resend. Status: {StatusCode}, Body: {Body}", response.StatusCode, error);
            response.EnsureSuccessStatusCode();
        }
    }
}
