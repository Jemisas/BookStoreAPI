namespace BookStoreAPI.Models.DTOs;

public class CsvBookRecord
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}
