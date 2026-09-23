using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookManager.Tests.Services.Jwt;

public class GenerateRefreshTokenTests : JwtServiceBase
{
    [Fact]
    public void GenerateRefreshToken_ShouldGenerateTokenWithRefreshTokenClaim_WhenUserProvided()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var jwtService = new JwtService(configuration);
        var user = new User { Id = 10, Username = "alice", Role = "User" };

        // Act
        var tokenString = jwtService.GenerateRefreshToken(user);

        // Assert
        tokenString.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        jwtToken.Claims.Should().Contain(c => c.Type == "token_type" && c.Value == "refresh");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "alice");
    }
}
