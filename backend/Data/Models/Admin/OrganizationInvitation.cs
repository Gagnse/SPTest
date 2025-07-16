using System;

namespace SpaceLogic.Data.Models.Admin
{
    public class OrganizationInvitation
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string? Department { get; set; }

        public string? Location { get; set; }

        public string Token { get; set; } = null!;

        public Guid InvitedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string Status { get; set; } = "pending";

        // Navigation
        public Organization? Organization { get; set; }

        public User? Inviter { get; set; }
    }
}
