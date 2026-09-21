using BookManager.Data;
using BookManager.DTOs.Books;
using BookManager.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Books
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Books
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Book> CreateAsync(CreateBookDto dto, CancellationToken ct = default)
    {
        var book = new Book
        {
            Title = dto.Title.Trim(),
            Author = dto.Author.Trim(),
            Price = dto.Price,
            Category = dto.Category.Trim(),
            Stock = dto.Stock
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync(ct);

        return book;
    }

    public async Task UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct = default)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Book with id {id} not found.");

        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();
        book.Price = dto.Price;
        book.Category = dto.Category.Trim();
        book.Stock = dto.Stock;

        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Book with id {id} not found.");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(ct);
    }
}
