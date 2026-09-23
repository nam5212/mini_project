using BookManager.Controllers;

namespace BookManager.Tests.Controllers.Books;

public abstract class BookControllerBase
{
    protected readonly IBookService _bookService;
    protected readonly BookController _controller;
    protected readonly HttpContext _httpContext;

    protected BookControllerBase()
    {
        _bookService = Substitute.For<IBookService>();
        _controller = new BookController(_bookService);

        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }
}
