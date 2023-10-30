using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities.Dtos;

public class UserDto
{
 
    [Required]
    public string Username { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
}
