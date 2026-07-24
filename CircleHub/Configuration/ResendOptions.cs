namespace CircleHub.Configuration;

public class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; init; } = string.Empty;

    public string From { get; init; } = string.Empty;
}
