namespace BookManager.Tests.Services.Books;

public class GetByIdAsyncTests : BookServiceBase
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedBookResponseDto_WhenBookExists()
    {
        // Arrange
        var book = new Book
        {
            Id = 1,
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Price = 55.0m,
            Category = "Architecture",
            Stock = 7
        };

        _bookRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(book);

        // Act
        var result = await _bookService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Domain-Driven Design");
        result.Author.Should().Be("Eric Evans");
        result.Price.Should().Be(55.0m);
        result.Category.Should().Be("Architecture");
        result.Stock.Should().Be(7);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        _bookRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Book?)null);

        // Act
        var result = await _bookService.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
