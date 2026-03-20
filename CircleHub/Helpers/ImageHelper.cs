using CircleHub.Models;

namespace CircleHub.Helpers
{
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
    }
}
