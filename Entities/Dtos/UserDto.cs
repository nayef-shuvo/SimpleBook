using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities.Dtos;

public class UserDto
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$", ErrorMessage = "Password must contain at least one letter, one digit, and one special character.")]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }
    
    [EmailAddress]
    [Required]
    public string Email { get; set; }
}
