namespace BookManager.Tests.Services.Books;

public class GetAllAsyncTests : BookServiceBase
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedPagedResultDto_WhenBooksExist()
    {
        // Arrange
        var books = new List<Book>
        {
            new() { Id = 1, Title = "Clean Code", Author = "Robert Martin", Price = 45.0m, Category = "Tech", Stock = 10 },
            new() { Id = 2, Title = "Refactoring", Author = "Martin Fowler", Price = 50.0m, Category = "Tech", Stock = 5 }
        };

        _bookRepository.GetAllAsync(null, null, null, null, 1, 10, Arg.Any<CancellationToken>())
            .Returns((books, 2));

        // Act
        var result = await _bookService.GetAllAsync();

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Items[0].Id.Should().Be(1);
        result.Items[0].Title.Should().Be("Clean Code");
        result.Items[1].Id.Should().Be(2);
        result.Items[1].Title.Should().Be("Refactoring");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyPagedResult_WhenRepositoryReturnsEmptyList()
    {
        // Arrange
        _bookRepository.GetAllAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<decimal?>(), Arg.Any<decimal?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((new List<Book>(), 0));

        // Act
        var result = await _bookService.GetAllAsync("non-existent");

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAllAsync_ShouldPassFilterParametersToRepository_WhenFiltersProvided()
    {
        // Arrange
        var search = "clean";
        var sort = "price_desc";
        decimal? minPrice = 10m;
        decimal? maxPrice = 100m;
        var pageIndex = 1;
        var pageSize = 10;

        _bookRepository.GetAllAsync(search, sort, minPrice, maxPrice, pageIndex, pageSize, Arg.Any<CancellationToken>())
            .Returns((new List<Book>(), 0));

        // Act
        await _bookService.GetAllAsync(search, sort, minPrice, maxPrice, pageIndex, pageSize);

        // Assert
        await _bookRepository.Received(1).GetAllAsync(search, sort, minPrice, maxPrice, pageIndex, pageSize, Arg.Any<CancellationToken>());
    }
}
