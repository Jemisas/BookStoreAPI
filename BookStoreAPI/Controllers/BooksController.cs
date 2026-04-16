using BookStoreAPI.Models.DTOs;
using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers;

[ApiController]
[Route("api/books")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<BookResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null,
        [FromQuery] string? authorName = null)
    {
        var result = await _bookService.GetAllAsync(page, pageSize, title, authorName);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookResponseDto>> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponseDto>> Create([FromBody] CreateBookDto dto)
    {
        var created = await _bookService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BookResponseDto>> Update(Guid id, [FromBody] UpdateBookDto dto)
    {
        var updated = await _bookService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _bookService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("validation/{isbn}")]
    public async Task<ActionResult<object>> ValidateIsbn(string isbn)
    {
        var isValid = await _bookService.ValidateIsbnAsync(isbn);
        return Ok(new { isValid });
    }

    [HttpPost("upload")]
    [HttpPost("masive")]
    public async Task<ActionResult<List<BookResponseDto>>> CreateMassive(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "A CSV file is required." });

        using var stream = file.OpenReadStream();
        var created = await _bookService.CreateMassiveAsync(stream);
        return Ok(created);
    }
}
