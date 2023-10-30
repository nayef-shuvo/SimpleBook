using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBook.Data;
using SimpleBook.Entities;
using SimpleBook.Entities.Dtos;

namespace SimpleBook.Controllers;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    public UsersController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserDto user)
    {
        var _user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == user.Username);
        var newUSer = new User
        {
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Password = user.Password,
        };
        return Ok(newUSer);
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        return Ok();
    }

    [HttpGet]
    [Route("profile")]
    [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> GetProfile()
    {
        return Ok();
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile()
    {
        return Ok();
    }
}
