namespace BookManager.Tests.Services.Books;

public class DeleteAsyncTests : BookServiceBase
{
    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDelete_WhenBookExists()
    {
        // Arrange
        var existingBook = new Book { Id = 3, Title = "Book to delete", Author = "Author" };
        _bookRepository.GetByIdAsync(3, Arg.Any<CancellationToken>())
            .Returns(existingBook);

        // Act
        await _bookService.DeleteAsync(3);

        // Assert
        await _bookRepository.Received(1).DeleteAsync(existingBook, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenBookNotFound()
    {
        // Arrange
        _bookRepository.GetByIdAsync(404, Arg.Any<CancellationToken>())
            .Returns((Book?)null);

        // Act
        Func<Task> act = async () => await _bookService.DeleteAsync(404);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Book with id 404 not found.");
        await _bookRepository.DidNotReceive().DeleteAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }
}
