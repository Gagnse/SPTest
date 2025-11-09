using backend.Services.Common;
using backend.Services.Interfaces;
using backend.Models.DTOs.Email;

namespace backend.Services
{
    /// <summary>
    /// Stub implementation of email service
    /// TODO: Replace with actual Gmail SMTP implementation when ready
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task<IResult<bool>> SendEmailAsync(EmailRequest request)
        {
            // TODO: Implement actual email sending via Gmail SMTP
            _logger.LogInformation("📧 [EMAIL STUB] Would send email:");
            _logger.LogInformation("   To: {To}", request.To);
            _logger.LogInformation("   Subject: {Subject}", request.Subject);
            _logger.LogInformation("   Body: {Body}", request.Body);
            _logger.LogInformation("   IsHtml: {IsHtml}", request.IsHtml);

            return Task.FromResult<IResult<bool>>(Result<bool>.Success(true));
        }

        public Task<IResult<bool>> SendInvitationEmailAsync(InvitationEmailData data)
        {
            // TODO: Implement actual email sending with invitation template
            _logger.LogInformation("📧 [INVITATION EMAIL STUB]");
            _logger.LogInformation("   To: {Email} ({FirstName} {LastName})", 
                data.RecipientEmail, data.RecipientFirstName, data.RecipientLastName);
            _logger.LogInformation("   From: {Inviter} at {Organization}", 
                data.InviterName, data.OrganizationName);
            _logger.LogInformation("   Invitation URL: {Url}", data.InvitationUrl);
            _logger.LogInformation("   Token: {Token}", data.InvitationToken);
            _logger.LogInformation("   Expires: {ExpiresAt}", data.ExpiresAt);
            _logger.LogInformation("");
            _logger.LogInformation("🔗 COPY THIS TOKEN TO ACCEPT INVITATION: {Token}", data.InvitationToken);

            // Simulate email content
            var emailBody = $@"
Hello {data.RecipientFirstName} {data.RecipientLastName},

{data.InviterName} has invited you to join {data.OrganizationName}!

Click the link below to accept the invitation and create your account:
{data.InvitationUrl}

Or use this token: {data.InvitationToken}

This invitation expires on {data.ExpiresAt:yyyy-MM-dd HH:mm:ss UTC}.

If you didn't expect this invitation, you can safely ignore this email.
";

            _logger.LogInformation("📝 Email Body:\n{Body}", emailBody);

            return Task.FromResult<IResult<bool>>(Result<bool>.Success(true));
        }

        public Task<IResult<bool>> SendPasswordResetEmailAsync(PasswordResetEmailData data)
        {
            // TODO: Implement actual email sending with password reset template
            _logger.LogInformation("📧 [PASSWORD RESET EMAIL STUB]");
            _logger.LogInformation("   To: {Email} ({Name})", data.RecipientEmail, data.RecipientName);
            _logger.LogInformation("   Reset URL: {Url}", data.ResetUrl);
            _logger.LogInformation("   Token: {Token}", data.ResetToken);
            _logger.LogInformation("   Expires: {ExpiresAt}", data.ExpiresAt);
            _logger.LogInformation("");
            _logger.LogInformation("🔗 COPY THIS TOKEN TO RESET PASSWORD: {Token}", data.ResetToken);

            // Simulate email content
            var emailBody = $@"
Hello {data.RecipientName},

We received a request to reset your password.

Click the link below to reset your password:
{data.ResetUrl}

Or use this token: {data.ResetToken}

This link expires on {data.ExpiresAt:yyyy-MM-dd HH:mm:ss UTC}.

If you didn't request a password reset, you can safely ignore this email.
";

            _logger.LogInformation("📝 Email Body:\n{Body}", emailBody);

            return Task.FromResult<IResult<bool>>(Result<bool>.Success(true));
        }
    }
}