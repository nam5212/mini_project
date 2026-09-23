namespace BookManager.Tests.Controllers.Books;

public class CreateTests : BookControllerBase
{
    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenValidRequest()
    {
        // Arrange
        var createDto = new CreateBookDto
        {
            Title = "Refactoring",
            Author = "Martin Fowler",
            Price = 49.99m,
            Category = "Software",
            Stock = 8
        };

        var responseDto = new BookResponseDto
        {
            Id = 15,
            Title = "Refactoring",
            Author = "Martin Fowler",
            Price = 49.99m,
            Category = "Software",
            Stock = 8
        };

        _bookService.CreateAsync(createDto, Arg.Any<CancellationToken>())
            .Returns(responseDto);

        // Act
        var result = await _controller.Create(createDto, CancellationToken.None);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(BookController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(15);
        createdResult.Value.Should().BeEquivalentTo(responseDto);
    }
}
