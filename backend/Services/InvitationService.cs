using backend.Services.Common;
using backend.Services.Interfaces;
using backend.Models.DTOs.Organization;
using backend.Models.DTOs.Email;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;
using System.Security.Cryptography;

namespace backend.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly AdminDbContext _context;
        private readonly ILogger<InvitationService> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public InvitationService(
            AdminDbContext context,
            ILogger<InvitationService> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IResult<OrganizationInvitationDto>> SendInvitationAsync(
            InviteUserToOrganizationRequest request,
            Guid organizationId,
            Guid invitedBy)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (existingUser != null)
                {
                    return Result<OrganizationInvitationDto>.Failure("User with this email already exists");
                }

                // Check if there's already a pending invitation
                var existingInvitation = await _context.OrganizationInvitations
                    .FirstOrDefaultAsync(i => i.Email.ToLower() == request.Email.ToLower() 
                        && i.OrganizationId == organizationId 
                        && i.Status == "pending");

                if (existingInvitation != null)
                {
                    return Result<OrganizationInvitationDto>.Failure("Pending invitation already exists for this email");
                }

                // Get organization and inviter details
                var organization = await _context.Organizations.FindAsync(organizationId);
                var inviter = await _context.Users.FindAsync(invitedBy);

                if (organization == null || inviter == null)
                {
                    return Result<OrganizationInvitationDto>.Failure("Organization or inviter not found");
                }

                // Generate secure token
                var token = GenerateSecureToken();

                // Create invitation
                var invitation = new OrganizationInvitation
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    Email = request.Email,
                    FirstName = "", // Will be provided when accepting
                    LastName = "",
                    Role = request.Role ?? "Member",
                    Department = request.Department,
                    Location = request.Location,
                    Token = token,
                    InvitedBy = invitedBy,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(request.ExpiryDays),
                    Status = "pending"
                };

                _context.OrganizationInvitations.Add(invitation);

                // Create email token
                var emailToken = new EmailToken
                {
                    Id = Guid.NewGuid(),
                    UserId = null, // New user invitation
                    Email = request.Email,
                    Token = token,
                    TokenType = "invitation",
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedAt = DateTime.UtcNow
                };

                _context.EmailTokens.Add(emailToken);
                await _context.SaveChangesAsync();

                // Send email
                var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5173";
                var invitationUrl = $"{baseUrl}/accept-invitation?token={token}";

                var emailData = new InvitationEmailData
                {
                    RecipientEmail = request.Email,
                    RecipientFirstName = "", // Unknown until they accept
                    RecipientLastName = "",
                    InviterName = $"{inviter.FirstName} {inviter.LastName}",
                    OrganizationName = organization.Name,
                    InvitationToken = token,
                    InvitationUrl = invitationUrl,
                    ExpiresAt = invitation.ExpiresAt
                };

                await _emailService.SendInvitationEmailAsync(emailData);

                var dto = new OrganizationInvitationDto
                {
                    Id = invitation.Id,
                    OrganizationId = invitation.OrganizationId,
                    OrganizationName = organization.Name,
                    Email = invitation.Email,
                    Role = invitation.Role,
                    Status = invitation.Status,
                    CreatedAt = invitation.CreatedAt,
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedBy = invitation.InvitedBy,
                    CreatedByName = $"{inviter.FirstName} {inviter.LastName}"
                };

                _logger.LogInformation("Sent invitation to {Email} for organization {OrganizationId}", 
                    request.Email, organizationId);

                return Result<OrganizationInvitationDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invitation to {Email}", request.Email);
                return Result<OrganizationInvitationDto>.Failure("Error sending invitation");
            }
        }

        public async Task<IResult<bool>> AcceptInvitationAsync(AcceptInvitationRequest request)
        {
            try
            {
                // Validate token
                var emailToken = await _context.EmailTokens
                    .FirstOrDefaultAsync(t => t.Token == request.Token && t.TokenType == "invitation");

                if (emailToken == null)
                {
                    return Result<bool>.Failure("Invalid invitation token");
                }

                if (emailToken.IsExpired)
                {
                    return Result<bool>.Failure("Invitation has expired");
                }

                if (emailToken.IsUsed)
                {
                    return Result<bool>.Failure("Invitation has already been used");
                }

                // Get invitation
                var invitation = await _context.OrganizationInvitations
                    .FirstOrDefaultAsync(i => i.Token == request.Token && i.Status == "pending");

                if (invitation == null)
                {
                    return Result<bool>.Failure("Invitation not found");
                }

                // Check if user already exists (edge case)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == invitation.Email.ToLower());

                if (existingUser != null)
                {
                    return Result<bool>.Failure("User with this email already exists");
                }

                // Hash password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = invitation.Email,
                    Password = hashedPassword,
                    OrganizationId = invitation.OrganizationId,
                    OrgRole = invitation.Role,
                    Department = invitation.Department,
                    Location = invitation.Location,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastActive = DateTime.UtcNow
                };

                _context.Users.Add(user);

                // Create organization-user relationship
                var orgUser = new OrganizationUser
                {
                    OrganizationId = invitation.OrganizationId,
                    UserId = user.Id,
                    JoinedAt = DateTime.UtcNow
                };

                _context.OrganizationUsers.Add(orgUser);

                // Mark invitation as accepted
                invitation.Status = "accepted";
                invitation.FirstName = request.FirstName;
                invitation.LastName = request.LastName;

                // Mark email token as used
                emailToken.UsedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} accepted invitation and joined organization {OrganizationId}", 
                    user.Id, invitation.OrganizationId);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting invitation");
                return Result<bool>.Failure("Error accepting invitation");
            }
        }

        public async Task<IResult<OrganizationInvitationDto>> ValidateInvitationTokenAsync(string token)
        {
            try
            {
                var invitation = await _context.OrganizationInvitations
                    .Include(i => i.Organization)
                    .Include(i => i.Inviter)
                    .FirstOrDefaultAsync(i => i.Token == token);

                if (invitation == null)
                {
                    return Result<OrganizationInvitationDto>.Failure("Invalid invitation token");
                }

                var dto = new OrganizationInvitationDto
                {
                    Id = invitation.Id,
                    OrganizationId = invitation.OrganizationId,
                    OrganizationName = invitation.Organization?.Name ?? "",
                    Email = invitation.Email,
                    Role = invitation.Role,
                    Status = invitation.Status,
                    CreatedAt = invitation.CreatedAt,
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedBy = invitation.InvitedBy,
                    CreatedByName = invitation.Inviter != null 
                        ? $"{invitation.Inviter.FirstName} {invitation.Inviter.LastName}"
                        : "Unknown"
                };

                return Result<OrganizationInvitationDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating invitation token");
                return Result<OrganizationInvitationDto>.Failure("Error validating invitation");
            }
        }

        public async Task<IResult<bool>> CancelInvitationAsync(Guid invitationId, Guid cancelledBy, Guid organizationId)
        {
            try
            {
                var invitation = await _context.OrganizationInvitations
                    .FirstOrDefaultAsync(i => i.Id == invitationId && i.OrganizationId == organizationId);

                if (invitation == null)
                {
                    return Result<bool>.Failure("Invitation not found");
                }

                if (invitation.Status != "pending")
                {
                    return Result<bool>.Failure("Can only cancel pending invitations");
                }

                invitation.Status = "cancelled";
                await _context.SaveChangesAsync();

                _logger.LogInformation("Invitation {InvitationId} cancelled by {UserId}", invitationId, cancelledBy);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling invitation {InvitationId}", invitationId);
                return Result<bool>.Failure("Error cancelling invitation");
            }
        }

        public async Task<IResult<OrganizationInvitationDto>> ResendInvitationAsync(Guid invitationId, Guid organizationId)
        {
            try
            {
                var invitation = await _context.OrganizationInvitations
                    .Include(i => i.Organization)
                    .Include(i => i.Inviter)
                    .FirstOrDefaultAsync(i => i.Id == invitationId && i.OrganizationId == organizationId);

                if (invitation == null)
                {
                    return Result<OrganizationInvitationDto>.Failure("Invitation not found");
                }

                if (invitation.Status != "pending")
                {
                    return Result<OrganizationInvitationDto>.Failure("Can only resend pending invitations");
                }

                // Generate new token
                var newToken = GenerateSecureToken();
                invitation.Token = newToken;
                invitation.ExpiresAt = DateTime.UtcNow.AddDays(7);

                // Create new email token
                var emailToken = new EmailToken
                {
                    Id = Guid.NewGuid(),
                    UserId = null,
                    Email = invitation.Email,
                    Token = newToken,
                    TokenType = "invitation",
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedAt = DateTime.UtcNow
                };

                _context.EmailTokens.Add(emailToken);
                await _context.SaveChangesAsync();

                // Resend email
                var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5173";
                var invitationUrl = $"{baseUrl}/accept-invitation?token={newToken}";

                var emailData = new InvitationEmailData
                {
                    RecipientEmail = invitation.Email,
                    RecipientFirstName = "",
                    RecipientLastName = "",
                    InviterName = $"{invitation.Inviter!.FirstName} {invitation.Inviter.LastName}",
                    OrganizationName = invitation.Organization!.Name,
                    InvitationToken = newToken,
                    InvitationUrl = invitationUrl,
                    ExpiresAt = invitation.ExpiresAt
                };

                await _emailService.SendInvitationEmailAsync(emailData);

                var dto = new OrganizationInvitationDto
                {
                    Id = invitation.Id,
                    OrganizationId = invitation.OrganizationId,
                    OrganizationName = invitation.Organization.Name,
                    Email = invitation.Email,
                    Role = invitation.Role,
                    Status = invitation.Status,
                    CreatedAt = invitation.CreatedAt,
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedBy = invitation.InvitedBy,
                    CreatedByName = $"{invitation.Inviter.FirstName} {invitation.Inviter.LastName}"
                };

                _logger.LogInformation("Resent invitation {InvitationId}", invitationId);
                return Result<OrganizationInvitationDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending invitation {InvitationId}", invitationId);
                return Result<OrganizationInvitationDto>.Failure("Error resending invitation");
            }
        }

        public async Task<IResult<List<OrganizationInvitationDto>>> GetPendingInvitationsAsync(Guid organizationId)
        {
            try
            {
                var invitations = await _context.OrganizationInvitations
                    .Where(i => i.OrganizationId == organizationId && i.Status == "pending" && i.ExpiresAt > DateTime.UtcNow)
                    .Include(i => i.Inviter)
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new OrganizationInvitationDto
                    {
                        Id = i.Id,
                        OrganizationId = i.OrganizationId,
                        OrganizationName = i.Organization!.Name,
                        Email = i.Email,
                        Role = i.Role,
                        Status = i.Status,
                        CreatedAt = i.CreatedAt,
                        ExpiresAt = i.ExpiresAt,
                        CreatedBy = i.InvitedBy,
                        CreatedByName = i.Inviter != null 
                            ? $"{i.Inviter.FirstName} {i.Inviter.LastName}"
                            : "Unknown"
                    })
                    .ToListAsync();

                return Result<List<OrganizationInvitationDto>>.Success(invitations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending invitations for organization {OrganizationId}", organizationId);
                return Result<List<OrganizationInvitationDto>>.Failure("Error retrieving invitations");
            }
        }

        public async Task<IResult<int>> CleanupExpiredInvitationsAsync()
        {
            try
            {
                var expiredInvitations = await _context.OrganizationInvitations
                    .Where(i => i.Status == "pending" && i.ExpiresAt < DateTime.UtcNow)
                    .ToListAsync();

                foreach (var invitation in expiredInvitations)
                {
                    invitation.Status = "expired";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Marked {Count} invitations as expired", expiredInvitations.Count);
                return Result<int>.Success(expiredInvitations.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired invitations");
                return Result<int>.Failure("Error cleaning up invitations");
            }
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}