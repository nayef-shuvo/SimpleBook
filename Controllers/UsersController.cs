using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> Register([FromBody] UserDto request)
    {

        /// Validation
        if (!ModelState.IsValid) return BadRequest(ModelState);

        /// Unique username check
        var temp = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (temp != null) return BadRequest("This username is already used.");

        /// Unique email check
        temp = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (temp != null) return BadRequest("This email is already used");

        /// create new user
        string passwordHash = GenerateHash(request.Password);
        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            Password = passwordHash,
        };
        var userRole = new UserRole
        {
            Id = newUser.Id,
            Role = request.Username == "Admin" ? RoleType.Admin : RoleType.User,
        };

        await dbContext.UserRoles.AddAsync(userRole);
        await dbContext.Users.AddAsync(newUser);
        await dbContext.SaveChangesAsync();

        return Ok(newUser);
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

        /// User does not exist
        if (user == null) return BadRequest("Invalid username or password");

        /// Password verification
        if (!Verify(request.Password, user.Password)) return BadRequest("Invalid username or password");

        /// Generate token
        var userRole = await dbContext.UserRoles.FirstOrDefaultAsync(x => x.Id == user.Id);
        var jwt = GenerateToken(user, userRole.Role);
        return Ok(new { Bearer = jwt });
    }

    /// For testing purpose
    [HttpGet]
    [Route("profile")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetProfile()
    {
        string claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == claimedId);
        if (user == null) return BadRequest("User not found");
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await dbContext.Users.ToListAsync());
    }

    [HttpPut("profile")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> UpdateProfile(UpdateDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        string claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == claimedId);
        if (user == null) return BadRequest("User not found");

        // Check if username is unique
        var isUsernameTaken = await dbContext.Users.AnyAsync(x => x.Username == request.Username && x.Id != claimedId);
        if (isUsernameTaken) return BadRequest("This username is already used.");

        // Check if email is unique
        var isEmailTaken = await dbContext.Users.AnyAsync(x => x.Email == request.Email && x.Id != claimedId);
        if (isEmailTaken) return BadRequest("This email is already used");

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Username = request.Username;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("id")]
    [Authorize(Roles = "Admin, User")]
    public IActionResult Delete(string id)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.Id == id);
        if (user == null) return BadRequest("User not found");
        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
        return Ok();
    }

    [HttpPost("change-password")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        string claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == claimedId);
        if (user == null) return BadRequest("User not found");

        if (!Verify(request.OldPassword, user.Password)) return BadRequest("Invalid password");

        if (request.OldPassword == request.NewPassword) return BadRequest("New password cannot be the same as old password");

        user.Password = GenerateHash(request.NewPassword);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.Username == request.Username);
        if (user == null) return BadRequest("User not found");

        var newPassword = "12345678Aa!";
        user.Password = GenerateHash(newPassword);
        await dbContext.SaveChangesAsync();

        var response = $"Your new password is {newPassword}. Please change it immediately after login.";

        return Ok(response);
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

    private string GenerateToken(User user, RoleType role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, role.ToString()),
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