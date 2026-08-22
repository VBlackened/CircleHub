namespace CircleHub.Services.Email
{
    public class EmailRequest
    {
        public required IEnumerable<string> Recipients { get; init; }

        public required string Subject { get; init; }

        public required string HtmlBody { get; init; }

        public string? ReplyToEmail { get; init; }

        public string? FromName { get; init; }
    }
}
