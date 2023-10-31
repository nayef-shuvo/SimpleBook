using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities.Dtos;

public class LoginDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
