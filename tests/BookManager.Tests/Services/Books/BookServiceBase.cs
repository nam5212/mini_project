namespace BookManager.Tests.Services.Books;

public abstract class BookServiceBase
{
    protected readonly IBookRepository _bookRepository;
    protected readonly BookService _bookService;

    protected BookServiceBase()
    {
        _bookRepository = Substitute.For<IBookRepository>();
        _bookService = new BookService(_bookRepository);
    }
}
