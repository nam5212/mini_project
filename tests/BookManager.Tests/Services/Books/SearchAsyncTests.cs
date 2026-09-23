namespace BookManager.Tests.Services.Books;

public class SearchAsyncTests : BookServiceBase
{
    [Fact]
    public async Task SearchAsync_ShouldCallGetAllAsyncWithKeyword_WhenKeywordProvided()
    {
        // Arrange
        var books = new List<Book>
        {
            new() { Id = 1, Title = "C# in Depth", Author = "Jon Skeet", Price = 45m, Category = ".NET", Stock = 10 }
        };

        _bookRepository.GetAllAsync(search: "C#", ct: Arg.Any<CancellationToken>())
            .Returns(books);

        // Act
        var result = await _bookService.SearchAsync("C#");

        // Assert
        result.Should().HaveCount(1);
        result[0].Title.Should().Be("C# in Depth");
    }
}
