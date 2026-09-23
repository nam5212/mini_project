using System.Security.Claims;

namespace BookManager.Tests.Services.Auth;

public class RefreshAsyncTests : AuthServiceBase
{
    [Fact]
    public async Task RefreshAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValidAndUserExists()
    {
        // Arrange
        var token = "valid-refresh-token";
        var user = new User { Id = 1, Username = "tokenuser", Role = "User" };
        var claims = new[] { new Claim(ClaimTypes.Name, "tokenuser") };
        var identity = new ClaimsIdentity(claims, "jwt");
        var principal = new ClaimsPrincipal(identity);

        _jwtService.ValidateRefreshToken(token).Returns(principal);
        _userRepository.GetByUsernameAsync("tokenuser", Arg.Any<CancellationToken>()).Returns(user);
        _jwtService.GenerateToken(user).Returns("new-access-token");
        _jwtService.GenerateRefreshToken(user).Returns("new-refresh-token");

        // Act
        var result = await _authService.RefreshAsync(token);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
        result.User.Username.Should().Be("tokenuser");
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnNull_WhenRefreshTokenValidationFails()
    {
        // Arrange
        var invalidToken = "invalid-token";
        _jwtService.ValidateRefreshToken(invalidToken).Returns((ClaimsPrincipal?)null);

        // Act
        var result = await _authService.RefreshAsync(invalidToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnNull_WhenPrincipalHasNoUsernameIdentity()
    {
        // Arrange
        var token = "token-without-name";
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        _jwtService.ValidateRefreshToken(token).Returns(principal);

        // Act
        var result = await _authService.RefreshAsync(token);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnNull_WhenUserInTokenDoesNotExistInDatabase()
    {
        // Arrange
        var token = "token-deleted-user";
        var claims = new[] { new Claim(ClaimTypes.Name, "deleteduser") };
        var identity = new ClaimsIdentity(claims, "jwt");
        var principal = new ClaimsPrincipal(identity);

        _jwtService.ValidateRefreshToken(token).Returns(principal);
        _userRepository.GetByUsernameAsync("deleteduser", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _authService.RefreshAsync(token);

        // Assert
        result.Should().BeNull();
    }
}
