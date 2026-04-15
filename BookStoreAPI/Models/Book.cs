using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAPI.Models;

public class Book
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? CoverUrl { get; set; }

    public int PublicationYear { get; set; }

    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }

    public Author Author { get; set; } = null!;
}
