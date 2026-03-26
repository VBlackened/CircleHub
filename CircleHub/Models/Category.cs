using CircleHub.Client.Models;
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

    public CategoryDTO ToDTO()
    {
        CategoryDTO dto = new CategoryDTO
        {
            Id = this.Id,
            Name = this.Name
        };
        foreach (Contact contact in Contacts)
        {
            //prevent circular reference
            contact.Categories.Clear();
            dto.Contacts.Add(contact.ToDTO());
        }
        return dto;
    }
}
