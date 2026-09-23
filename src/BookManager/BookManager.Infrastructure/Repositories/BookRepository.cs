using BookManager.Infrastructure.Data;
using BookManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using BookManager.Application.Interfaces;
namespace BookManager.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Book> Items, int TotalCount)> GetAllAsync(
        string? search = null,
        string? sort = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = _context.Books
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(x =>
                x.Title.Contains(keyword) ||
                x.Author.Contains(keyword) ||
                x.Category.Contains(keyword));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(x => x.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync(ct);

        query = sort?.Trim().ToLowerInvariant() switch
        {
            "price_desc" => query.OrderByDescending(x => x.Price).ThenBy(x => x.Id),
            "price_asc" => query.OrderBy(x => x.Price).ThenBy(x => x.Id),
            _ => query.OrderBy(x => x.Id)
        };

        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
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
