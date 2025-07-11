// backend/Controllers/Shared/SharedDtos.cs
using System;

namespace backend.Controllers.Shared
{
    // User Data Transfer Object
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public Guid OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    // Organization Data Transfer Object
    public class OrganizationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinedAt { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
    }

    // Organization switch request
    public class SwitchOrganizationRequest
    {
        public Guid OrganizationId { get; set; }
    }

    // Organization switch response
    public class SwitchOrganizationResponse
    {
        public UserDto User { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
    }
}