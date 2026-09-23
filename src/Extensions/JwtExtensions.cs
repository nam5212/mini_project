using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BookManager.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = GetRequiredConfigurationValue(
            configuration,
            "JWT_SECRET",
            "Jwt:Key",
            "Jwt__Key",
            "JWT_KEY");

        var jwtIssuer = GetRequiredConfigurationValue(
            configuration,
            "JWT_ISSUER",
            "Jwt:Issuer",
            "Jwt__Issuer");

        var jwtAudience = GetRequiredConfigurationValue(
            configuration,
            "JWT_AUDIENCE",
            "Jwt:Audience",
            "Jwt__Audience");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
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
}
