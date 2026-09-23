namespace BookManager.Tests.Controllers.Books;

public class UpdateTests : BookControllerBase
{
    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var updateDto = new UpdateBookDto
        {
            Title = "Clean Code Revised",
            Author = "Robert C. Martin",
            Price = 45m,
            Category = "Programming",
            Stock = 20
        };

        // Act
        var result = await _controller.Update(1, updateDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        await _bookService.Received(1).UpdateAsync(1, updateDto, Arg.Any<CancellationToken>());
    }
}
