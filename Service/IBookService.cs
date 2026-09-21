using BookManager.DTOs.Books;
using BookManager.Models;

namespace BookManager.Services;

public interface IBookService
{
    Task<List<Book>> GetAllAsync(
        CancellationToken ct = default);

    Task<Book?> GetByIdAsync(
        int id,
        CancellationToken ct = default);

    Task<Book> CreateAsync(
        CreateBookDto dto,
        CancellationToken ct = default);

    Task UpdateAsync(
        int id,
        UpdateBookDto dto,
        CancellationToken ct = default);

    Task DeleteAsync(
        int id,
        CancellationToken ct = default);
}