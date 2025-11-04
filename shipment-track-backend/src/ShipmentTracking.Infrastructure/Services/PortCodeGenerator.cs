using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Application.Common.Interfaces;

namespace ShipmentTracking.Infrastructure.Services;

/// <summary>
/// Generates unique port codes based on port name and country, following UN/LOCODE standard.
/// Format: 2-letter country code + 3-letter location code (e.g., USNYC, CNSHA)
/// </summary>
public sealed class PortCodeGenerator : IPortCodeGenerator
{
    private const int MaxAttempts = 10;
    private readonly IApplicationDbContext _dbContext;

    // ISO 3166-1 alpha-2 country codes mapping
    private static readonly Dictionary<string, string> CountryCodeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "United States", "US" },
        { "USA", "US" },
        { "China", "CN" },
        { "Singapore", "SG" },
        { "Netherlands", "NL" },
        { "Germany", "DE" },
        { "United Arab Emirates", "AE" },
        { "UAE", "AE" },
        { "United Kingdom", "GB" },
        { "UK", "GB" },
        { "Japan", "JP" },
        { "Brazil", "BR" },
        { "Australia", "AU" },
        { "Jordan", "JO" },
        { "Syria", "SY" },
        { "Lebanon", "LB" },
        { "Egypt", "EG" },
        { "Saudi Arabia", "SA" },
        { "Iraq", "IQ" },
        { "Turkey", "TR" },
        { "Iran", "IR" },
        { "Pakistan", "PK" },
        { "India", "IN" },
        { "South Korea", "KR" },
        { "France", "FR" },
        { "Spain", "ES" },
        { "Italy", "IT" },
        { "Canada", "CA" },
        { "Mexico", "MX" },
    };

    public PortCodeGenerator(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateAsync(string portName, string country, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(portName))
        {
            throw new ArgumentException("Port name cannot be empty.", nameof(portName));
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be empty.", nameof(country));
        }

        var countryCode = GetCountryCode(country);
        var locationCode = GenerateLocationCode(portName);

        // Try the primary combination first
        var candidate = $"{countryCode}{locationCode}";
        if (!await CodeExistsAsync(candidate, cancellationToken))
        {
            return candidate;
        }

        // If collision, try variations
        for (var attempt = 1; attempt < MaxAttempts; attempt++)
        {
            candidate = $"{countryCode}{locationCode.Substring(0, 2)}{attempt}";
            if (!await CodeExistsAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        // Last resort: use random suffix
        var random = new Random();
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var suffix = random.Next(0, 999).ToString("D3");
            candidate = $"{countryCode}{suffix}";
            if (!await CodeExistsAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException($"Failed to generate unique port code for {portName}, {country} after {MaxAttempts * 2} attempts.");
    }

    private string GetCountryCode(string country)
    {
        var normalized = country.Trim();

        // Check if already a 2-letter code
        if (normalized.Length == 2 && normalized.All(char.IsLetter))
        {
            return normalized.ToUpperInvariant();
        }

        // Try to find in mapping
        if (CountryCodeMap.TryGetValue(normalized, out var code))
        {
            return code;
        }

        // Fallback: use first 2 letters of country name
        var letters = new string(normalized.Where(char.IsLetter).Take(2).ToArray());
        return letters.Length == 2 ? letters.ToUpperInvariant() : "XX";
    }

    private string GenerateLocationCode(string portName)
    {
        // Remove common port-related words
        var cleanName = portName
            .Replace("Port of", "", StringComparison.OrdinalIgnoreCase)
            .Replace("Port", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        // Extract first 3 letters (consonants preferred for readability)
        var letters = cleanName
            .Where(char.IsLetter)
            .Select(char.ToUpperInvariant)
            .ToList();

        if (letters.Count >= 3)
        {
            // Try to get consonants first for better readability
            var consonants = letters.Where(c => !"AEIOU".Contains(c)).ToList();
            if (consonants.Count >= 3)
            {
                return new string(consonants.Take(3).ToArray());
            }

            // If not enough consonants, use first 3 letters
            return new string(letters.Take(3).ToArray());
        }

        // Pad with X if not enough letters
        var code = new string(letters.ToArray()).PadRight(3, 'X');
        return code;
    }

    private async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
    {
        return await _dbContext.Ports
            .AsNoTracking()
            .AnyAsync(p => p.Code == code, cancellationToken);
    }
}
