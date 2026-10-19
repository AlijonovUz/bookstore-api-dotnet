using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstBackendApp.Contracts.Authors.Parameters;
using FirstBackendApp.Contracts.Authors.Requests;
using FirstBackendApp.Contracts.Authors.Responses;
using FirstBackendApp.Contracts.Common.Responses;
using FirstBackendApp.Services.Authors;

namespace FirstBackendApp.Controllers;

// Mualliflar API controlleri
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    // 1. GET: api/authors - Barcha mualliflarni olish
    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<PagedResponse<AuthorResponse>>> GetAllAsync([FromQuery] AuthorFilterParameters parameters)
    {
        var result = await _authorService.GetAllAsync(parameters);
        return Ok(result);
    }

    // 2. GET: api/authors/{authorId} - ID bo'yicha muallifni olish
    [HttpGet("{authorId:int}"), AllowAnonymous]
    public async Task<ActionResult<AuthorResponse>> GetByIdAsync([FromRoute] int authorId)
    {
        var author = await _authorService.GetByIdAsync(authorId);
        if (author is null) return NotFound();

        return Ok(author);
    }

    // 3. POST: api/authors - Yangi muallif qo'shish
    [HttpPost]
    public async Task<ActionResult<AuthorResponse>> CreateAsync([FromBody] CreateAuthorRequest request)
    {
        var createAuthor = await _authorService.CreateAsync(request);
        return CreatedAtAction("GetById", new { authorId = createAuthor.Id }, createAuthor);
    }

    // 4. PUT: api/authors/{authorId} - Muallif ma'lumotlarini to'liq yangilash
    [HttpPut("{authorId:int}")]
    public async Task<ActionResult<AuthorResponse>> UpdateAsync([FromRoute] int authorId, [FromBody] UpdateAuthorRequest request)
    {
        var updateAuthor = await _authorService.UpdateAsync(authorId, request);
        if (updateAuthor is null) return NotFound();
        return Ok(updateAuthor);
    }

    // 5. PATCH: api/authors/{authorId} - Muallif ma'lumotlarini qisman yangilash
    [HttpPatch("{authorId:int}")]
    public async Task<ActionResult<AuthorResponse>> PatchAsync([FromRoute] int authorId, [FromBody] PatchAuthorRequest request)
    {
        var patchAuthor = await _authorService.PatchAsync(authorId, request);
        if (patchAuthor is null) return NotFound();

        return Ok(patchAuthor);
    }

    // 6. DELETE: api/authors/{authorId} - Muallifni o'chirish
    [HttpDelete("{authorId:int}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int authorId)
    {
        var success = await _authorService.DeleteAsync(authorId);
        if (!success) return NotFound();

        return NoContent();
    }
}