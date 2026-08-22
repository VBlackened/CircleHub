namespace CircleHub.Configuration;

public class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; init; } = string.Empty;

    public string SystemFrom { get; init; } = string.Empty;

    public string ContactFrom { get; init; } = string.Empty;
}
