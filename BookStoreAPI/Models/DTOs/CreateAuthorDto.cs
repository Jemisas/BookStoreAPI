using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Models.DTOs;

public class CreateAuthorDto
{
    [Required]
    [MaxLength(200)]
    [DefaultValue("Juan Rulfo")]
    public string Name { get; set; } = string.Empty;
}
