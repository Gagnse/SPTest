using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Email
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string To { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = true;
    }

    public class InvitationEmailData
    {
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientFirstName { get; set; } = string.Empty;
        public string RecipientLastName { get; set; } = string.Empty;
        public string InviterName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string InvitationToken { get; set; } = string.Empty;
        public string InvitationUrl { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class PasswordResetEmailData
    {
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public string ResetUrl { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}