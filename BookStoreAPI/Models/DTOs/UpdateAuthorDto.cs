using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Models.DTOs;

public class UpdateAuthorDto
{
    [MaxLength(200)]
    [DefaultValue("Gabriel García Márquez")]
    public string? Name { get; set; }
}
