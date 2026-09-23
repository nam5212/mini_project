namespace BookManager.DTOs.Books;

public class BookResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Author { get; set; } = "";

    public decimal Price { get; set; }

    public string Category { get; set; } = "";

    public int Stock { get; set; }
}