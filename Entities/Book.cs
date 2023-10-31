using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities;

public class Book
{
    [Key]
    public string Id { get; private init; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(13, MinimumLength = 13)]
    [RegularExpression("^[0-9]*$", ErrorMessage = "ISBN must be all digits.")]
    public string Isbn { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Author { get; set; }

    [Required]
    public int Edition { get; set; }

    [Required]
    [Range(0, 100_000)]
    public decimal Price { get; set; }
}
