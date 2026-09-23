namespace BookManager.Tests.Services.Books;

public class CreateAsyncTests : BookServiceBase
{
    [Fact]
    public async Task CreateAsync_ShouldTrimFieldsAndSaveBook_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateBookDto
        {
            Title = "  The Pragmatic Programmer  ",
            Author = "  Andy Hunt  ",
            Price = 40.0m,
            Category = "  Software Engineering  ",
            Stock = 12
        };

        _bookRepository.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var book = callInfo.Arg<Book>();
                book.Id = 10;
                return book;
            });

        // Act
        var result = await _bookService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.Title.Should().Be("The Pragmatic Programmer");
        result.Author.Should().Be("Andy Hunt");
        result.Category.Should().Be("Software Engineering");
        result.Price.Should().Be(40.0m);
        result.Stock.Should().Be(12);

        await _bookRepository.Received(1).AddAsync(
            Arg.Is<Book>(b =>
                b.Title == "The Pragmatic Programmer" &&
                b.Author == "Andy Hunt" &&
                b.Category == "Software Engineering" &&
                b.Price == 40.0m &&
                b.Stock == 12),
            Arg.Any<CancellationToken>());
    }
}
