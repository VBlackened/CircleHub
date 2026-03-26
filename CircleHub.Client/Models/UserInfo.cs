namespace CircleHub.Client.Models
{
    public class UserInfo
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ProfilePictureUrl { get; set; }
    }
}
