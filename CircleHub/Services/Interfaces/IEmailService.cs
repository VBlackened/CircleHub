using CircleHub.Services.Email;

namespace CircleHub.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailRequest request);
}
