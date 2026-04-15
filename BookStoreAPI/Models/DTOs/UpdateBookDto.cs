using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Models.DTOs;

public class UpdateBookDto
{
    [MaxLength(300)]
    [DefaultValue("Cien años de soledad")]
    public string? Title { get; set; }

    [DefaultValue(1967)]
    public int? PublicationYear { get; set; }

    public Guid? AuthorId { get; set; }
}
