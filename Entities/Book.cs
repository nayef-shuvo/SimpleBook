using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities;

public class Book
{
    [Key]
    public string Id { get; private init; } = Guid.NewGuid().ToString();
    
    [StringLength(13)]
    public string Isbn { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public int Edition { get; set; }

    [Required]
    public float Price { get; set; }
    
    


    
    
}
