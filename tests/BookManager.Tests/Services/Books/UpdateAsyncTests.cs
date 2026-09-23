namespace BookManager.Tests.Services.Books;

public class UpdateAsyncTests : BookServiceBase
{
    [Fact]
    public async Task UpdateAsync_ShouldUpdatePropertiesAndCallRepository_WhenBookExists()
    {
        // Arrange
        var existingBook = new Book
        {
            Id = 5,
            Title = "Old Title",
            Author = "Old Author",
            Price = 20.0m,
            Category = "Old Category",
            Stock = 2
        };

        var updateDto = new UpdateBookDto
        {
            Title = "  New Title  ",
            Author = "  New Author  ",
            Price = 35.0m,
            Category = "  New Category  ",
            Stock = 15
        };

        _bookRepository.GetByIdAsync(5, Arg.Any<CancellationToken>())
            .Returns(existingBook);

        // Act
        await _bookService.UpdateAsync(5, updateDto);

        // Assert
        existingBook.Title.Should().Be("New Title");
        existingBook.Author.Should().Be("New Author");
        existingBook.Category.Should().Be("New Category");
        existingBook.Price.Should().Be(35.0m);
        existingBook.Stock.Should().Be(15);

        await _bookRepository.Received(1).UpdateAsync(existingBook, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenBookNotFound()
    {
        // Arrange
        var updateDto = new UpdateBookDto
        {
            Title = "Title",
            Author = "Author",
            Price = 30.0m,
            Category = "Category",
            Stock = 5
        };

        _bookRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Book?)null);

        // Act
        Func<Task> act = async () => await _bookService.UpdateAsync(999, updateDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Book with id 999 not found.");
        await _bookRepository.DidNotReceive().UpdateAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }
}
