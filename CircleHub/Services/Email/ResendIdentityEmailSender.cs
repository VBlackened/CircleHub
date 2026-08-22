using CircleHub.Configuration;
using CircleHub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Resend;

namespace CircleHub.Services.Email;

public class ResendIdentityEmailSender(IResend _resend, IOptions<ResendOptions> _options) : IEmailSender<ApplicationUser>
{
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var message = new EmailMessage
        {
            From = _options.Value.SystemFrom,
            Subject = "Confirm your CircleHub account",
            HtmlBody = $$"""
            <p>Hello {{user.FirstName}},</p>

            <p>
            Please confirm your email by clicking
            <a href="{{confirmationLink}}">here</a>.
            </p>
            """
        };

        message.To.Add(email);

        await _resend.EmailSendAsync(message);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var message = new EmailMessage
        {
            From = _options.Value.SystemFrom,
            Subject = "Reset your CircleHub password",
            HtmlBody = $$"""
            <p>Hello {{user.FirstName}},</p>

            <p>
            You requested a password reset. Use the following code to reset your password: <strong>{{resetCode}}</strong>
            </p>
            """
        };

        message.To.Add(email);
        await _resend.EmailSendAsync(message);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var message = new EmailMessage
        {
            From = _options.Value.SystemFrom,
            Subject = "Reset your CircleHub password",
            HtmlBody = $$"""
            <p>Hello {{user.FirstName}},</p>

            <p>
            You requested a password reset. Click the following link to reset your password: <a href="{{resetLink}}">Reset Password</a>
            </p>
            """
        };

        message.To.Add(email);
        await _resend.EmailSendAsync(message);
    }
}
