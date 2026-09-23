namespace BookManager.Tests.Controllers.Books;

public class GetByIdTests : BookControllerBase
{
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenBookExists()
    {
        // Arrange
        var book = new BookResponseDto
        {
            Id = 1,
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Price = 55m,
            Category = "Architecture",
            Stock = 7
        };

        _bookService.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(book);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(book);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _bookService.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((BookResponseDto?)null);

        // Act
        var result = await _controller.GetById(999, CancellationToken.None);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }
}
