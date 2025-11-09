using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models.DTOs.Organization;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    /// <summary>
    /// Manages user invitation lifecycle including sending, accepting, and validating invitations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationsController : BaseController
    {
        private readonly IInvitationService _invitationService;
        private readonly ILogger<InvitationsController> _logger;

        public InvitationsController(
            IInvitationService invitationService,
            ILogger<InvitationsController> logger)
        {
            _invitationService = invitationService;
            _logger = logger;
        }

        /// <summary>
        /// Send invitation to new user
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SendInvitation([FromBody] InviteUserToOrganizationRequest request)
        {
            // TODO: Phase 4 - Add [Permission("users", "invite")] attribute
            var invitedBy = GetCurrentUserId();
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "User {InvitedBy} sending invitation to {Email} for organization {OrgId}", 
                invitedBy, request.Email, organizationId);

            var result = await _invitationService.SendInvitationAsync(request, organizationId, invitedBy);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Accept invitation and create user account
        /// </summary>
        [HttpPost("{token}/accept")]
        [AllowAnonymous]
        public async Task<ActionResult> AcceptInvitation(
            [FromRoute] string token, 
            [FromBody] AcceptInvitationRequest request)
        {
            _logger.LogInformation("Invitation acceptance attempted with token");

            // Set the token from route parameter
            request.Token = token;
            
            var result = await _invitationService.AcceptInvitationAsync(request);
            return HandleServiceResult(result, "Account created successfully");
        }

        /// <summary>
        /// Validate invitation token
        /// </summary>
        [HttpGet("{token}/validate")]
        [AllowAnonymous]
        public async Task<ActionResult> ValidateInvitationToken([FromRoute] string token)
        {
            _logger.LogInformation("Validating invitation token");

            var result = await _invitationService.ValidateInvitationTokenAsync(token);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Cancel pending invitation
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> CancelInvitation([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("users", "invite")] attribute
            var userId = GetCurrentUserId();
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "User {UserId} cancelling invitation {InvitationId}", 
                userId, id);

            var result = await _invitationService.CancelInvitationAsync(id, userId, organizationId);
            return HandleServiceResult(result, "Invitation cancelled successfully");
        }

        /// <summary>
        /// Resend invitation email with new token
        /// </summary>
        [HttpPost("{id}/resend")]
        [Authorize]
        public async Task<ActionResult> ResendInvitation([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("users", "invite")] attribute
            var userId = GetCurrentUserId();
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "User {UserId} resending invitation {InvitationId}", 
                userId, id);

            var result = await _invitationService.ResendInvitationAsync(id, organizationId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Get all pending invitations for organization
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetPendingInvitations()
        {
            // TODO: Phase 4 - Add [Permission("users", "invite")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "Getting pending invitations for organization {OrgId}", 
                organizationId);

            var result = await _invitationService.GetPendingInvitationsAsync(organizationId);
            return HandleServiceResult(result);
        }
    }
}