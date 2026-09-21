using BookManager.Data;
using BookManager.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _context.Books
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
    }

    public async Task<Book?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        return await _context.Books
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Book> AddAsync(
        Book book,
        CancellationToken ct = default)
    {
        _context.Books.Add(book);

        await _context.SaveChangesAsync(ct);

        return book;
    }

    public async Task UpdateAsync(
        Book book,
        CancellationToken ct = default)
    {
        _context.Books.Update(book);

        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(
        Book book,
        CancellationToken ct = default)
    {
        _context.Books.Remove(book);

        await _context.SaveChangesAsync(ct);
    }
}