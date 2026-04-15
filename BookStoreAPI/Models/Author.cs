using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Models;

public class Author
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
