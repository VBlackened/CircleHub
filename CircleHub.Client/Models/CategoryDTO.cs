using System.ComponentModel.DataAnnotations;

namespace CircleHub.Client.Models;

public class CategoryDTO
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Category Name")]
    public string? Name { get; set; }
    public virtual ICollection<ContactDTO> Contacts { get; set; } = [];
}
