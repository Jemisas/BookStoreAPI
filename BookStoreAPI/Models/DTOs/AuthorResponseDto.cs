namespace BookStoreAPI.Models.DTOs;

public class AuthorResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<BookResponseDto> Books { get; set; } = new();
}
