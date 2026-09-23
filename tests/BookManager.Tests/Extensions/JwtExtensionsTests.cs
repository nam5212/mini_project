using BookManager.API.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookManager.Tests.Extensions;

public class JwtExtensionsTests
{
    [Fact]
    public void AddJwtAuthentication_ShouldRegisterAuthenticationAndJwtBearerOptions_WhenValidConfigurationProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "JWT_SECRET", "super_secret_jwt_key_that_is_at_least_32_bytes_long" },
            { "JWT_ISSUER", "BookManagerApp" },
            { "JWT_AUDIENCE", "BookManagerClients" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        services.AddJwtAuthentication(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authService = serviceProvider.GetService<IAuthenticationService>();
        authService.Should().NotBeNull();

        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        authOptions.DefaultAuthenticateScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        authOptions.DefaultChallengeScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);

        var jwtBearerOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);
        jwtBearerOptions.TokenValidationParameters.ValidIssuer.Should().Be("BookManagerApp");
        jwtBearerOptions.TokenValidationParameters.ValidAudience.Should().Be("BookManagerClients");
    }

    [Fact]
    public void AddJwtAuthentication_ShouldThrowInvalidOperationException_WhenSecretMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "JWT_ISSUER", "BookManagerApp" },
            { "JWT_AUDIENCE", "BookManagerClients" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var act = () => services.AddJwtAuthentication(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Missing required configuration value*");
    }
}
