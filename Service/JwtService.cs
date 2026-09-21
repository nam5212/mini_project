using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookManager.Models;
using Microsoft.IdentityModel.Tokens;

namespace BookManager.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        static string GetConfigurationValue(IConfiguration configuration, params string[] keys)
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

        static string GetSecretValue(IConfiguration configuration, params string[] keys)
        {
            var value = GetConfigurationValue(configuration, keys);
            return value.Length >= 16 ? value : string.Empty;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var secretKey = GetSecretValue(
                _configuration,
                "JWT_SECRET",
                "Jwt:Key",
                "Jwt__Key",
                "JWT_KEY")
            .Length > 0
            ? GetSecretValue(
                _configuration,
                "JWT_SECRET",
                "Jwt:Key",
                "Jwt__Key",
                "JWT_KEY")
            : "ThisIsASecretKeyForBookManager1234567890";

        var issuer = GetConfigurationValue(
                _configuration,
                "JWT_ISSUER",
                "Jwt:Issuer",
                "Jwt__Issuer")
            .Length > 0
            ? GetConfigurationValue(
                _configuration,
                "JWT_ISSUER",
                "Jwt:Issuer",
                "Jwt__Issuer")
            : "BookApi";

        var audience = GetConfigurationValue(
                _configuration,
                "JWT_AUDIENCE",
                "Jwt:Audience",
                "Jwt__Audience")
            .Length > 0
            ? GetConfigurationValue(
                _configuration,
                "JWT_AUDIENCE",
                "Jwt:Audience",
                "Jwt__Audience")
            : "BookApiClient";

        var expireMinutesValue = GetConfigurationValue(
            _configuration,
            "JWT_EXPIRES_MINUTES",
            "JWT_EXPIRE_MINUTES",
            "Jwt:ExpireMinutes");

        var expireMinutes = int.TryParse(expireMinutesValue, out var parsedExpireMinutes)
            ? parsedExpireMinutes
            : 60;

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
}