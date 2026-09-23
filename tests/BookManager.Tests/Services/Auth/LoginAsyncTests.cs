namespace BookManager.Tests.Services.Auth;

public class LoginAsyncTests : AuthServiceBase
{
    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var rawPassword = "ValidPassword123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);
        var user = new User
        {
            Id = 1,
            Username = "validuser",
            PasswordHash = hashedPassword,
            Role = "User"
        };
        var loginDto = new LoginDto { Username = "validuser", Password = rawPassword };

        _userRepository.GetByUsernameAsync(loginDto.Username, Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtService.GenerateToken(user).Returns("mocked-access-token");
        _jwtService.GenerateRefreshToken(user).Returns("mocked-refresh-token");

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("mocked-access-token");
        result.RefreshToken.Should().Be("mocked-refresh-token");
        result.User.Should().NotBeNull();
        result.User.Id.Should().Be(1);
        result.User.Username.Should().Be("validuser");
        result.User.Role.Should().Be("User");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "unknownuser", Password = "AnyPassword" };
        _userRepository.GetByUsernameAsync(loginDto.Username, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = hashedPassword,
            Role = "User"
        };
        var loginDto = new LoginDto { Username = "testuser", Password = "WrongPassword" };

        _userRepository.GetByUsernameAsync(loginDto.Username, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }
}
