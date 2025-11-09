using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    [Table("email_tokens")]
    public class EmailToken
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid? UserId { get; set; } // NULL for new user invitations

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("token")]
        public string Token { get; set; } = null!;

        [Column("token_type")]
        public string TokenType { get; set; } = null!; // 'invitation', 'password_reset'

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("used_at")]
        public DateTime? UsedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }

        // Computed properties
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public bool IsUsed => UsedAt.HasValue;
        public bool IsValid => !IsExpired && !IsUsed;
    }
}