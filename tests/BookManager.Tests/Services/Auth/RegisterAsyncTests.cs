namespace BookManager.Tests.Services.Auth;

public class RegisterAsyncTests : AuthServiceBase
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateAndReturnUser_WhenUsernameIsAvailable()
    {
        // Arrange
        var dto = new RegisterDto { Username = "newuser", Password = "Password123!" };
        _userRepository.UsernameExistsAsync(dto.Username, Arg.Any<CancellationToken>())
            .Returns(false);
        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var user = callInfo.Arg<User>();
                user.Id = 1;
                return user;
            });

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Username.Should().Be("newuser");
        result.Role.Should().Be("User");

        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Username == "newuser" && !string.IsNullOrEmpty(u.PasswordHash) && u.Role == "User"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowInvalidOperationException_WhenUsernameAlreadyExists()
    {
        // Arrange
        var dto = new RegisterDto { Username = "existinguser", Password = "Password123!" };
        _userRepository.UsernameExistsAsync(dto.Username, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Username already exists.");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterAsync_ShouldTrimUsername_WhenUsernameHasLeadingOrTrailingWhitespace()
    {
        // Arrange
        var dto = new RegisterDto { Username = "  spaceduser  ", Password = "Password123!" };
        _userRepository.UsernameExistsAsync(dto.Username, Arg.Any<CancellationToken>())
            .Returns(false);
        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var user = callInfo.Arg<User>();
                user.Id = 2;
                return user;
            });

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Username.Should().Be("spaceduser");
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Username == "spaceduser"),
            Arg.Any<CancellationToken>());
    }
}
