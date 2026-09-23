using System.ComponentModel.DataAnnotations;

namespace BookManager.DTOs.Books;

public class UpdateBookDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = "";

    [Range(0, 1000000)]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = "";

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}