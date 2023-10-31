using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBook.Data;
using SimpleBook.Entities;
using SimpleBook.Entities.Dtos;

namespace SimpleBook.Controllers;

[ApiController]
[Route("books")]
public class BookController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    public BookController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet("id")]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        var book = await dbContext.Books.FindAsync(id);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await dbContext.Books.ToListAsync();
        return Ok(books);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Create([FromBody] BookDto book)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var bookExists = await dbContext.Books.AnyAsync(b => b.Isbn == book.Isbn);
        if (bookExists) return BadRequest("This isbn already exists.");

        var newBook = new Book
        {
            Isbn = book.Isbn,
            Title = book.Title,
            Edition = book.Edition,
            Price = book.Price,
        };

        await dbContext.Books.AddAsync(newBook);
        await dbContext.SaveChangesAsync();

        return Ok(newBook);
    }

    [HttpPut("id")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Update([FromQuery] string id, [FromBody] BookDto bookDto)
    {
        if (!ModelState.IsValid) return BadRequest();

        var book = await dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

        if (book == null) return NotFound();

        book.Isbn = bookDto.Isbn;
        book.Title = bookDto.Title;
        book.Edition = bookDto.Edition;
        book.Price = bookDto.Price;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("id")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Delete([FromQuery] string id)
    {
        var book = await dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
        if (book == null) return NotFound();

        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

}

