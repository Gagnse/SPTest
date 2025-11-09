using backend.Services.Common;
using backend.Models.DTOs.Email;

namespace backend.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends a generic email
        /// </summary>
        Task<IResult<bool>> SendEmailAsync(EmailRequest request);

        /// <summary>
        /// Sends an invitation email to a new user
        /// </summary>
        Task<IResult<bool>> SendInvitationEmailAsync(InvitationEmailData data);

        /// <summary>
        /// Sends a password reset email
        /// </summary>
        Task<IResult<bool>> SendPasswordResetEmailAsync(PasswordResetEmailData data);
    }
}