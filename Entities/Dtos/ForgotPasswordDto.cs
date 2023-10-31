using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities.Dtos;

public class ForgotPasswordDto
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }

}

