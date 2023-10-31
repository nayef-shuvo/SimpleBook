using System.ComponentModel.DataAnnotations;

namespace SimpleBook.Entities.Dtos;

public class BookDto
{
    [Required]
    [StringLength(13, MinimumLength = 13)]
    [RegularExpression("^[0-9]*$", ErrorMessage = "ISBN must be all digits.")]
    public string Isbn { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public int Edition { get; set; }

    [Required]
    public decimal Price { get; set; }
}