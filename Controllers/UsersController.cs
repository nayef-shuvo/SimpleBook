using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using SimpleBook.Data;
using SimpleBook.Entities;
using SimpleBook.Entities.Dtos;

namespace SimpleBook.Controllers;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IConfiguration config;
    public UsersController(ApplicationDbContext dbContext, IConfiguration config)
    {
        this.dbContext = dbContext;
        this.config = config;
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserDto user)
    {

        /// Validation
        if (!ModelState.IsValid) return BadRequest(ModelState);

        /// Unique username check
        var temp = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == user.Username);
        if (temp != null) return BadRequest("This username is already used.");

        /// Unique email check
        temp = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        if (temp != null) return BadRequest("This email is already used");

        /// create new user
        string passwordHash = GenerateHash(user.Password);
        var newUser = new User
        {
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Password = passwordHash,
        };

        await dbContext.Users.AddAsync(newUser);
        await dbContext.SaveChangesAsync();

        return Ok(newUser);
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == login.Username);

        /// User does not exist
        if (user == null) return BadRequest("Invalid username or password");

        /// Password verification
        if (!Verify(login.Password, user.Password)) return BadRequest("Invalid username or password");

        /// Generate token
        var jwt = GenerateToken(user, UserType.User);
        return Ok(new { token = jwt });
    }

    [HttpGet]
    [Route("profile")]
    // [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await dbContext.Users.ToListAsync());
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile()
    {
        return Ok();
    }

    private string GenerateHash(string password)
    {
        var key = Encoding.UTF8.GetBytes(config["ShaKey:Key"]!);
        using var hmac = new HMACSHA512(key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    private bool Verify(string password, string hash)
    {
        var key = Encoding.UTF8.GetBytes(config["ShaKey:Key"]!);
        using var hmac = new HMACSHA512(key);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(Convert.FromBase64String(hash));
    }



    private string GenerateToken(User user, UserType userType)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, userType.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
