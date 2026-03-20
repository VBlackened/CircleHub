using CircleHub.Data;
using System.ComponentModel.DataAnnotations;

namespace CircleHub.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Category Name")]
    public string? Name { get; set; }

    [Required]
    public string? AppUserId { get; set; }
    public virtual ApplicationUser? AppUser { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = [];
}
