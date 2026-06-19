using CircleHub.Models;
using System.Text.RegularExpressions;

namespace CircleHub.Helpers;

public static class ImageHelper
{
    public static readonly string DefaultProfilePictureUrl = "/Images/DefaultProfilePicture.svg";

    public static async Task<ImageUpload> GetImageUploadAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        byte[] data = memoryStream.ToArray();

        if (memoryStream.Length > 1 * 1024 * 1024)
        {
            throw new Exception("The image is too large.");
        }

        ImageUpload imageUpload = new()
        {
            Id = Guid.NewGuid(),
            Data = data,
            Type = file.ContentType

        };

        return imageUpload;
    }

    public static ImageUpload GetImageUploadFromUrl(string dataUrl)
    {
        // regex pattern to match data URLs of the format: data:[<mediatype>][;base64],<data>
        GroupCollection matchGroups = Regex.Match(dataUrl, @"data:(?<type>.+?);base64,(?<data>.+)").Groups;

        if (matchGroups.ContainsKey("type") && matchGroups.ContainsKey("data"))
        {
            //returns sth like "image/png" or "image/jpeg"
            string contentType = matchGroups["type"].Value;
            byte[] data = Convert.FromBase64String(matchGroups["data"].Value);

            if (data.Length <= 5 * 1024 * 1024)
            {
                ImageUpload imageUpload = new()
                {
                    Id = Guid.NewGuid(),
                    Data = data,
                    Type = contentType
                };

                return imageUpload;
            }
        }
            throw new IOException("Data URL was either invalid or file is too large.");
    }
}
