using BookManager.Application.DTOs.Books;
using BookManager.Application.DTOs.Common;
using BookManager.Application.Services;
using BookManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.API.Controllers;

[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<BookResponseDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _bookService.GetAllAsync(search, sort, minPrice, maxPrice, pageIndex, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponseDto>> GetById(int id, CancellationToken ct)
    {
        var book = await _bookService.GetByIdAsync(id, ct);

        if (book is null)
           return NotFound(new { message = $"Book with ID {id} not found." });

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponseDto>> Create(
        [FromBody] CreateBookDto dto,
        CancellationToken ct)
    {
        var book = await _bookService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateBookDto dto,
        CancellationToken ct)
    {
        await _bookService.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _bookService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<BookResponseDto>>> Search(
    [FromQuery] string keyword,
    CancellationToken ct)
    {
    var books = await _bookService.SearchAsync(keyword, ct);
    return Ok(books);
    }


}
