namespace BookManager.Tests.Controllers.Books;

public class DeleteTests : BookControllerBase
{
    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleteIsSuccessful()
    {
        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        await _bookService.Received(1).DeleteAsync(1, Arg.Any<CancellationToken>());
    }
}
