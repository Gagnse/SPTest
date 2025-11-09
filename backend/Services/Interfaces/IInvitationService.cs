using backend.Services.Common;
using backend.Models.DTOs.Organization;

namespace backend.Services.Interfaces
{
    public interface IInvitationService
    {
        Task<IResult<OrganizationInvitationDto>> SendInvitationAsync(
            InviteUserToOrganizationRequest request, 
            Guid organizationId, 
            Guid invitedBy);

        Task<IResult<bool>> AcceptInvitationAsync(AcceptInvitationRequest request);

        Task<IResult<OrganizationInvitationDto>> ValidateInvitationTokenAsync(string token);

        Task<IResult<bool>> CancelInvitationAsync(Guid invitationId, Guid cancelledBy, Guid organizationId);

        Task<IResult<OrganizationInvitationDto>> ResendInvitationAsync(Guid invitationId, Guid organizationId);

        Task<IResult<List<OrganizationInvitationDto>>> GetPendingInvitationsAsync(Guid organizationId);

        Task<IResult<int>> CleanupExpiredInvitationsAsync();
    }
}