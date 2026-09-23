using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookManager.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

using BookManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
namespace BookManager.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var secretKey = GetAccessTokenSecret();
        var issuer = GetIssuer();
        var audience = GetAudience();

        var expireMinutesValue = GetOptionalConfigurationValue(
            _configuration,
            "JWT_EXPIRES_MINUTES",
            "JWT_EXPIRE_MINUTES",
            "Jwt:ExpireMinutes");

        var expireMinutes = int.TryParse(expireMinutesValue, out var parsedExpireMinutes)
            ? parsedExpireMinutes
            : 15;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var secretKey = GetRefreshTokenSecret();
        var issuer = GetIssuer();
        var audience = GetAudience();

        var expireDaysValue = GetOptionalConfigurationValue(
            _configuration,
            "JWT_REFRESH_EXPIRES_DAYS",
            "JWT_REFRESH_EXPIRE_DAYS",
            "Jwt:RefreshExpiresDays");

        var expireDays = int.TryParse(expireDaysValue, out var parsed) ? parsed : 7;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("token_type", "refresh")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expireDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateRefreshToken(string token)
    {
        var secretKey = GetRefreshTokenSecret();
        var issuer = GetIssuer();
        var audience = GetAudience();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Ensure token is a JWT and has the expected token_type claim
            if (validatedToken is JwtSecurityToken jwt &&
                jwt.Payload.TryGetValue("token_type", out var typeObj) &&
                typeObj?.ToString() == "refresh")
            {
                return principal;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private string GetAccessTokenSecret()
    {
        return GetRequiredConfigurationValue(
            _configuration,
            "JWT_SECRET",
            "Jwt:Key",
            "Jwt__Key",
            "JWT_KEY");
    }

    private string GetRefreshTokenSecret()
    {
        var refreshSecret = GetOptionalConfigurationValue(
            _configuration,
            "JWT_REFRESH_SECRET",
            "Jwt:RefreshKey",
            "Jwt__RefreshKey",
            "JWT_REFRESH_KEY");

        if (!string.IsNullOrWhiteSpace(refreshSecret))
        {
            return refreshSecret;
        }

        // Fallback an toàn: nếu chưa cấu hình RefreshKey riêng, tự sinh key riêng biệt từ Access Secret Key
        var accessSecret = GetAccessTokenSecret();
        return $"{accessSecret}_refresh_secret_key_salt";
    }

    private string GetIssuer()
    {
        return GetRequiredConfigurationValue(
            _configuration,
            "JWT_ISSUER",
            "Jwt:Issuer",
            "Jwt__Issuer");
    }

    private string GetAudience()
    {
        return GetRequiredConfigurationValue(
            _configuration,
            "JWT_AUDIENCE",
            "Jwt:Audience",
            "Jwt__Audience");
    }

    private static string GetRequiredConfigurationValue(IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        throw new InvalidOperationException($"Missing required configuration value. Set one of: {string.Join(", ", keys)}.");
    }

    private static string GetOptionalConfigurationValue(IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return string.Empty;
    }
}
