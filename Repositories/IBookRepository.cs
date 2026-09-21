using BookManager.Models;

namespace BookManager.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(
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