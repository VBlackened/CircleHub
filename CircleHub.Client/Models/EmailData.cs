using System.ComponentModel.DataAnnotations;

namespace CircleHub.Client.Models;

public class EmailData
{
    public required string Recipients { get; set; }

    [Length(5, 100, ErrorMessage = "Subject must be between 5 and 100 characters.")]
    public required string Subject { get; set; }

    [Length(10, 600, ErrorMessage = "Body must be between 10 and 600 characters.")]
    public required string Body { get; set; }
}
