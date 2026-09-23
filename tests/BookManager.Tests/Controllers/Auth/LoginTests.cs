namespace BookManager.Tests.Controllers.Auth;

public class LoginTests : AuthControllerBase
{
    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = new LoginDto { Username = "alice", Password = "Password123!" };
        var authResponse = new AuthResponseDto
        {
            AccessToken = "access-token-123",
            RefreshToken = "refresh-token-123",
            User = new AuthUserDto { Id = 1, Username = "alice", Role = "User" }
        };

        _authService.LoginAsync(dto, Arg.Any<CancellationToken>()).Returns(authResponse);

        // Act
        var result = await _controller.Login(dto, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(authResponse);
    }

    [Fact]
    public async Task Login_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var dto = new LoginDto { Username = "alice", Password = "WrongPassword" };

        _authService.LoginAsync(dto, Arg.Any<CancellationToken>()).Returns((AuthResponseDto?)null);

        // Act
        Func<Task> act = async () => await _controller.Login(dto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid username or password.");
    }
}
