using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Models.DTOs;

public class CreateBookDto
{
    [Required]
    [MaxLength(20)]
    [DefaultValue("9788437607351")]
    public string Isbn { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    [DefaultValue("Pedro Páramo")]
    public string Title { get; set; } = string.Empty;

    [DefaultValue(1955)]
    public int PublicationYear { get; set; }

    [Required]
    public Guid AuthorId { get; set; }
}
