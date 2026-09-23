namespace BookManager.Tests.Services.Jwt;

public class ValidateRefreshTokenTests : JwtServiceBase
{
    [Fact]
    public void ValidateRefreshToken_ShouldReturnClaimsPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var jwtService = new JwtService(configuration);
        var user = new User { Id = 10, Username = "alice", Role = "User" };
        var tokenString = jwtService.GenerateRefreshToken(user);

        // Act
        var principal = jwtService.ValidateRefreshToken(tokenString);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity.Should().NotBeNull();
        principal.Identity!.Name.Should().Be("alice");
    }

    [Fact]
    public void ValidateRefreshToken_ShouldReturnNull_WhenTokenIsTamperedOrInvalid()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var jwtService = new JwtService(configuration);

        // Act
        var result = jwtService.ValidateRefreshToken("invalid.tampered.token");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ValidateRefreshToken_ShouldReturnNull_WhenTokenIsAccessTokenInsteadOfRefreshToken()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var jwtService = new JwtService(configuration);
        var user = new User { Id = 10, Username = "alice", Role = "User" };
        var accessToken = jwtService.GenerateToken(user);

        // Act
        var result = jwtService.ValidateRefreshToken(accessToken);

        // Assert
        result.Should().BeNull();
    }
}
