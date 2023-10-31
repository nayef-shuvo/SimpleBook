using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities;

public class User
{
    [Key]
    public string Id { get; private init; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Username { get; set; }
    

    /// Hash of Actual Password    
    [Required]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    public DateTime DateRegistered { get; private init; } = DateTime.Now;
}
