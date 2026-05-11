using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CircleHub.Data;
using CircleHub.Client.Models;
using CircleHub.Helpers;

namespace CircleHub.Models;

public class Contact
{
    private DateTimeOffset _created;

    public int Id { get; set; }

    [Required]
    [Display(Name = "First Name")]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
    public string? FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
    public string? LastName { get; set; }

    [NotMapped]
    public string? FullNameTag { get { return $"{FirstName} {LastName}"; } }

    [Display(Name = "Birthday")]
    [DataType(DataType.Date)]
    public DateOnly? BirthDate { get; set; }

    [Required]
    [Display(Name = "Address")]
    public string? Address1 { get; set; }

    [Display(Name = "Address 2")]
    public string? Address2 { get; set; }

    [Required]
    [Display(Name = "City")]
    public string? City { get; set; }

    [Required]
    [Display(Name = "Postal Code")]
    [DataType(DataType.PostalCode)]
    public string? PostalCode { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTimeOffset Created
    {
        get => _created;
        set => _created = value.ToUniversalTime();
    }

    [Required]
    public string? AppUserId { get; set; }
    public virtual ApplicationUser? AppUser { get; set; }

    public Guid? ImageId { get; set; }
    public virtual ImageUpload? Image { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = [];

    public ContactDTO ToDTO()
    {
        ContactDTO dto = new ContactDTO
        {
            Id = this.Id,
            FirstName = this.FirstName,
            LastName = this.LastName,
            BirthDate = this.BirthDate,
            Address1 = this.Address1,
            Address2 = this.Address2,
            City = this.City,
            PostalCode = this.PostalCode,
            Email = this.Email,
            PhoneNumber = this.PhoneNumber,
            Created = this.Created,
            ProfileImageUrl = this.ImageId.HasValue ? $"/uploads/{ImageId}" : ImageHelper.DefaultProfilePictureUrl
        };

        foreach (Category category in Categories)
        {
            //prevent circular reference
            category.Contacts.Clear();
            dto.Categories.Add(category.ToDTO());
        }

        return dto;
    }


}
