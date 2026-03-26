using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CircleHub.Client.Models;

public class ContactDTO
{
    private DateTimeOffset _created;
    private DateTimeOffset? _birthDate;

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
    public DateTimeOffset? BirthDate
    {
        get => _birthDate;
        set => _birthDate = value?.ToUniversalTime();
    }

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

    public string? ProfileImageUrl { get; set; }

    public virtual ICollection<CategoryDTO> Categories { get; set; } = [];
}
