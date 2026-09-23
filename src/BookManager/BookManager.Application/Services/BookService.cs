using BookManager.Application.DTOs.Books;
using BookManager.Domain.Entities;
using BookManager.Application.Interfaces;
using BookManager.Application.Interfaces;

namespace BookManager.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<BookResponseDto>> GetAllAsync(
        string? search = null,
        string? sort = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken ct = default)
    {
        var books = await _bookRepository.GetAllAsync(search, sort, minPrice, maxPrice, ct);
        return books.Select(MapToResponseDto).ToList();
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
        var books = await _bookRepository.GetAllAsync(keyword, ct: ct);
        return books.Select(MapToResponseDto).ToList();
    }

  
}
