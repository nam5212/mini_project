namespace BookManager.Tests.Controllers.Books;

public class GetAllTests : BookControllerBase
{
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithListOfBooks()
    {
        // Arrange
        var books = new List<BookResponseDto>
        {
            new() { Id = 1, Title = "C# in Depth", Author = "Jon Skeet", Price = 45m, Category = ".NET", Stock = 10 },
            new() { Id = 2, Title = "Clean Architecture", Author = "Robert Martin", Price = 50m, Category = "Software", Stock = 5 }
        };

        _bookService.GetAllAsync(null, null, null, null, Arg.Any<CancellationToken>())
            .Returns(books);

        // Act
        var result = await _controller.GetAll(null, null, null, null, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task GetAll_ShouldPassFiltersToService_WhenFiltersAreProvided()
    {
        // Arrange
        var search = "Clean";
        var sort = "price_asc";
        decimal? minPrice = 10m;
        decimal? maxPrice = 60m;

        _bookService.GetAllAsync(search, sort, minPrice, maxPrice, Arg.Any<CancellationToken>())
            .Returns(new List<BookResponseDto>());

        // Act
        var result = await _controller.GetAll(search, sort, minPrice, maxPrice, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        await _bookService.Received(1).GetAllAsync(search, sort, minPrice, maxPrice, Arg.Any<CancellationToken>());
    }
}
