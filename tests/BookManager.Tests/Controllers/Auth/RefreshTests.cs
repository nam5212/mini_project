namespace BookManager.Tests.Controllers.Auth;

public class RefreshTests : AuthControllerBase
{
    [Fact]
    public async Task Refresh_ShouldReturnOk_WhenRefreshTokenIsValid()
    {
        // Arrange
        var dto = new RefreshRequestDto { RefreshToken = "valid-refresh-token" };
        var authResponse = new AuthResponseDto
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            User = new AuthUserDto { Id = 1, Username = "alice", Role = "User" }
        };

        _authService.RefreshAsync(dto.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(authResponse);

        // Act
        var result = await _controller.Refresh(dto, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(authResponse);
    }

    [Fact]
    public async Task Refresh_ShouldThrowUnauthorizedAccessException_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var dto = new RefreshRequestDto { RefreshToken = "invalid-token" };

        _authService.RefreshAsync(dto.RefreshToken, Arg.Any<CancellationToken>())
            .Returns((AuthResponseDto?)null);

        // Act
        Func<Task> act = async () => await _controller.Refresh(dto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token.");
    }
}
