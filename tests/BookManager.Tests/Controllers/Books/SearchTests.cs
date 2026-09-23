namespace BookManager.Tests.Controllers.Books;

public class SearchTests : BookControllerBase
{
    [Fact]
    public async Task Search_ShouldReturnOk_WithListOfMatchingBooks()
    {
        // Arrange
        var keyword = "patterns";
        var matchingBooks = new List<BookResponseDto>
        {
            new() { Id = 1, Title = "Design Patterns", Author = "GoF", Price = 55m, Category = "Architecture", Stock = 10 }
        };

        _bookService.SearchAsync(keyword, Arg.Any<CancellationToken>())
            .Returns(matchingBooks);

        // Act
        var result = await _controller.Search(keyword, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(matchingBooks);
    }
}
