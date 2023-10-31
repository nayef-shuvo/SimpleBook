using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities.Dtos;

public class ChangePasswordDto
{
    [Required]
    [MinLength(8)]
    public string OldPassword { get; set; } = null!;

    [Required]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$", ErrorMessage = "Password must contain at least one letter, one digit, and one special character.")]

    public string NewPassword { get; set; } = null!;
}
