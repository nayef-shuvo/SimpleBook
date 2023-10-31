using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities.Dtos;

public class UpdateDto
{
    [Required   ]
    public string Username { get; set; }
    
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    public string FullName { get; set; }    
}
