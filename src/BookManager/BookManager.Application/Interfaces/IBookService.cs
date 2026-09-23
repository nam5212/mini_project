using BookManager.Application.DTOs.Books;
using BookManager.Application.DTOs.Common;

namespace BookManager.Application.Interfaces;

public interface IBookService
{
    Task<PagedResultDto<BookResponseDto>> GetAllAsync(
        string? search = null,
        string? sort = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken ct = default);

    Task<BookResponseDto?> GetByIdAsync(
        int id,
        CancellationToken ct = default);

    Task<BookResponseDto> CreateAsync(
        CreateBookDto dto,
        CancellationToken ct = default);

    Task UpdateAsync(
        int id,
        UpdateBookDto dto,
        CancellationToken ct = default);

    Task DeleteAsync(
        int id,
        CancellationToken ct = default);

    Task<List<BookResponseDto>> SearchAsync(
        string keyword,
        CancellationToken ct = default);

}
