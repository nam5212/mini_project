using BookManager.Application.DTOs.Common;

namespace BookManager.Tests.Controllers.Books;

public class GetAllTests : BookControllerBase
{
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithPagedBooks()
    {
        // Arrange
        var pagedResult = new PagedResultDto<BookResponseDto>
        {
            Items = new List<BookResponseDto>
            {
                new() { Id = 1, Title = "C# in Depth", Author = "Jon Skeet", Price = 45m, Category = ".NET", Stock = 10 },
                new() { Id = 2, Title = "Clean Architecture", Author = "Robert Martin", Price = 50m, Category = "Software", Stock = 5 }
            },
            TotalCount = 2,
            PageIndex = 1,
            PageSize = 10
        };

        _bookService.GetAllAsync(null, null, null, null, 1, 10, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        // Act
        var result = await _controller.GetAll(null, null, null, null, 1, 10, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(pagedResult);
    }

    [Fact]
    public async Task GetAll_ShouldPassFiltersToService_WhenFiltersAreProvided()
    {
        // Arrange
        var search = "Clean";
        var sort = "price_asc";
        decimal? minPrice = 10m;
        decimal? maxPrice = 60m;
        var pageIndex = 1;
        var pageSize = 10;

        _bookService.GetAllAsync(search, sort, minPrice, maxPrice, pageIndex, pageSize, Arg.Any<CancellationToken>())
            .Returns(new PagedResultDto<BookResponseDto>());

        // Act
        var result = await _controller.GetAll(search, sort, minPrice, maxPrice, pageIndex, pageSize, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        await _bookService.Received(1).GetAllAsync(search, sort, minPrice, maxPrice, pageIndex, pageSize, Arg.Any<CancellationToken>());
    }
}
