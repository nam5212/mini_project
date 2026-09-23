using BookManager.Application.DTOs.Books;
using BookManager.Application.DTOs.Common;
using BookManager.Domain.Entities;
using BookManager.Application.Interfaces;

namespace BookManager.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<PagedResultDto<BookResponseDto>> GetAllAsync(
        string? search = null,
        string? sort = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;

        var (books, totalCount) = await _bookRepository.GetAllAsync(search, sort, minPrice, maxPrice, pageIndex, pageSize, ct);

        return new PagedResultDto<BookResponseDto>
        {
            Items = books.Select(MapToResponseDto).ToList(),
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<BookResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct);
        return book is null ? null : MapToResponseDto(book);
    }

    public async Task<BookResponseDto> CreateAsync(CreateBookDto dto, CancellationToken ct = default)
    {
        var book = new Book
        {
            Title = dto.Title.Trim(),
            Author = dto.Author.Trim(),
            Price = dto.Price,
            Category = dto.Category.Trim(),
            Stock = dto.Stock
        };

        var createdBook = await _bookRepository.AddAsync(book, ct);
        return MapToResponseDto(createdBook);
    }

    public async Task UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Book with id {id} not found.");

        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();
        book.Price = dto.Price;
        book.Category = dto.Category.Trim();
        book.Stock = dto.Stock;

        await _bookRepository.UpdateAsync(book, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Book with id {id} not found.");

        await _bookRepository.DeleteAsync(book, ct);
    }

    private static BookResponseDto MapToResponseDto(Book book)
    {
        return new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            Category = book.Category,
            Stock = book.Stock
        };
    }

    public async Task<List<BookResponseDto>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        var (books, _) = await _bookRepository.GetAllAsync(keyword, pageIndex: 1, pageSize: 1000, ct: ct);
        return books.Select(MapToResponseDto).ToList();
    }

  
}
