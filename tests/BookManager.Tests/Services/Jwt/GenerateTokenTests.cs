using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookManager.Tests.Services.Jwt;

public class GenerateTokenTests : JwtServiceBase
{
    [Fact]
    public void GenerateToken_ShouldReturnValidJwtTokenWithCorrectClaims_WhenUserProvided()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var jwtService = new JwtService(configuration);
        var user = new User
        {
            Id = 42,
            Username = "johndoe",
            Role = "Admin"
        };

        // Act
        var tokenString = jwtService.GenerateToken(user);

        // Assert
        tokenString.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        jwtToken.Issuer.Should().Be(TestIssuer);
        jwtToken.Audiences.Should().Contain(TestAudience);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "42");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "johndoe");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public void GenerateToken_ShouldThrowInvalidOperationException_WhenSecretKeyMissing()
    {
        // Arrange
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            { "JWT_ISSUER", TestIssuer },
            { "JWT_AUDIENCE", TestAudience }
        });
        var jwtService = new JwtService(configuration);
        var user = new User { Id = 1, Username = "user", Role = "User" };

        // Act
        var act = () => jwtService.GenerateToken(user);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Missing required configuration value*");
    }
}
