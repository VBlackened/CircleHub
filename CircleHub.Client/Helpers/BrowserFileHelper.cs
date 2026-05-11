using Microsoft.AspNetCore.Components.Forms;

namespace CircleHub.Client.Helpers;

public class BrowserFileHelper
{
    public static readonly string DefaultContactImage = "/Images/DefaultProfilePicture.svg";

    public static int MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public static async Task<string> GetImageDataUrlAsync(IBrowserFile file)
    {
        if (file == null)
            return string.Empty;
        using Stream fileStream = file.OpenReadStream(MaxFileSize);
        using MemoryStream ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);

        byte[] imgBytes = ms.ToArray();
        string imageBase64 = Convert.ToBase64String(imgBytes);
        string dataUrl = $"data:{file.ContentType};base64,{imageBase64}";

        return dataUrl;
    }
}
