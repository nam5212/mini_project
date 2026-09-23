using BookManager.Domain.Entities;

namespace BookManager.Application.Interfaces;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(
        string? search = null,
        string? sort = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken ct = default);

    Task<Book?> GetByIdAsync(
        int id,
        CancellationToken ct = default);

    Task<Book> AddAsync(
        Book book,
        CancellationToken ct = default);

    Task UpdateAsync(
        Book book,
        CancellationToken ct = default);

    Task DeleteAsync(
        Book book,
        CancellationToken ct = default);
}
