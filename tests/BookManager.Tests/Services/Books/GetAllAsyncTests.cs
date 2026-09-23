namespace BookManager.Tests.Services.Books;

public class GetAllAsyncTests : BookServiceBase
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedResponseDtoList_WhenBooksExist()
    {
        // Arrange
        var books = new List<Book>
        {
            new() { Id = 1, Title = "Clean Code", Author = "Robert Martin", Price = 45.0m, Category = "Tech", Stock = 10 },
            new() { Id = 2, Title = "Refactoring", Author = "Martin Fowler", Price = 50.0m, Category = "Tech", Stock = 5 }
        };

        _bookRepository.GetAllAsync(null, null, null, null, Arg.Any<CancellationToken>())
            .Returns(books);

        // Act
        var result = await _bookService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[0].Title.Should().Be("Clean Code");
        result[1].Id.Should().Be(2);
        result[1].Title.Should().Be("Refactoring");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
    {
        // Arrange
        _bookRepository.GetAllAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<decimal?>(), Arg.Any<decimal?>(), Arg.Any<CancellationToken>())
            .Returns(new List<Book>());

        // Act
        var result = await _bookService.GetAllAsync("non-existent");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldPassFilterParametersToRepository_WhenFiltersProvided()
    {
        // Arrange
        var search = "clean";
        var sort = "price_desc";
        decimal? minPrice = 10m;
        decimal? maxPrice = 100m;

        _bookRepository.GetAllAsync(search, sort, minPrice, maxPrice, Arg.Any<CancellationToken>())
            .Returns(new List<Book>());

        // Act
        await _bookService.GetAllAsync(search, sort, minPrice, maxPrice);

        // Assert
        await _bookRepository.Received(1).GetAllAsync(search, sort, minPrice, maxPrice, Arg.Any<CancellationToken>());
    }
}
