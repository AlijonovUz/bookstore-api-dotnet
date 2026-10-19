using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstBackendApp.Contracts.Books.Parameters;
using FirstBackendApp.Contracts.Books.Requests;
using FirstBackendApp.Contracts.Books.Responses;
using FirstBackendApp.Contracts.Common.Responses;
using FirstBackendApp.Services.Books;
using Microsoft.AspNetCore.RateLimiting;

namespace FirstBackendApp.Controllers;

// Kitoblar API controlleri
[ApiController]
[EnableRateLimiting("fixed-by-ip")]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // 1. GET: api/books - Barcha kitoblarni olish
    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<PagedResponse<BookResponse>>> GetAllAsync([FromQuery] BookFilterParameters parameters)
    {
        var result = await _bookService.GetAllAsync(parameters);
        return Ok(result);
    }

    // 2. GET: api/books/{bookId} - ID bo'yicha kitobni olish
    [HttpGet("{bookId:int}"), AllowAnonymous]
    public async Task<ActionResult<BookResponse>> GetByIdAsync([FromRoute] int bookId)
    {
        var book = await _bookService.GetByIdAsync(bookId);
        if (book is null) return NotFound();

        return Ok(book);
    }

    // 3. POST: api/books - Yangi kitob qo'shish (rasm bilan birga multipart/form-data)
    [HttpPost]
    public async Task<ActionResult<BookResponse>> CreateAsync([FromForm] CreateBookRequest request)
    {
        var createBook = await _bookService.CreateAsync(request);
        return CreatedAtAction("GetById", new { bookId = createBook.Id }, createBook);
    }

    // 4. PUT: api/books/{bookId} - Kitob ma'lumotlarini to'liq yangilash
    [HttpPut("{bookId:int}")]
    public async Task<ActionResult<BookResponse>> UpdateAsync([FromRoute] int bookId, [FromForm] UpdateBookRequest request)
    {
        var updateBook = await _bookService.UpdateAsync(bookId, request);
        if (updateBook is null) return NotFound();

        return Ok(updateBook);
    }

    // 5. PATCH: api/books/{bookId} - Kitob ma'lumotlarini qisman yangilash
    [HttpPatch("{bookId:int}")]
    public async Task<ActionResult<BookResponse>> PatchAsync([FromRoute] int bookId, [FromForm] PatchBookRequest request)
    {
        var patchBook = await _bookService.PatchAsync(bookId, request);
        if (patchBook is null) return NotFound();

        return Ok(patchBook);
    }

    // 6. DELETE: api/books/{bookId} - Kitobni va uning rasmini o'chirish
    [HttpDelete("{bookId:int}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int bookId)
    {
        var success = await _bookService.DeleteAsync(bookId);
        if (!success) return NotFound();

        return NoContent();
    }
}