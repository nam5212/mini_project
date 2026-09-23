namespace BookManager.Tests.Controllers.Auth;

public class RegisterTests : AuthControllerBase
{
    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var dto = new RegisterDto { Username = "alice", Password = "Password123!" };
        var authUser = new AuthUserDto { Id = 1, Username = "alice", Role = "User" };

        _authService.RegisterAsync(dto, Arg.Any<CancellationToken>()).Returns(authUser);

        // Act
        var result = await _controller.Register(dto, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(authUser);
    }
}
