using CircleHub.Configuration;
using CircleHub.Data;
using CircleHub.Services.Interfaces;
using Microsoft.Extensions.Options;
using Resend;

namespace CircleHub.Services.Email;

public class ResendEmailService(IResend _resend, IOptions<ResendOptions> _options) : IEmailService
{
    public async Task SendEmailAsync(EmailRequest request)
    {
        var message = new EmailMessage
        {
            From = new EmailAddress
            {
                Email = _options.Value.ContactFrom,
                DisplayName = request.FromName
            },
            To = EmailAddressList.From(request.Recipients),
            Subject = request.Subject,
            HtmlBody = request.HtmlBody
        };

        if (!string.IsNullOrWhiteSpace(request.ReplyToEmail))
        {
            message.ReplyTo = request.ReplyToEmail;
        }

        await _resend.EmailSendAsync(message);
    }
}
