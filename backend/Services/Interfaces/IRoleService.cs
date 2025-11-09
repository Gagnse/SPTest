using backend.Services.Common;
using backend.Models.DTOs.Role;

namespace backend.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IResult<RoleListDto>> GetOrganizationRolesAsync(Guid organizationId);

        Task<IResult<RoleWithPermissionsDto>> GetRoleByIdAsync(Guid roleId, Guid organizationId);

        Task<IResult<RoleDto>> CreateRoleAsync(CreateRoleRequest request, Guid organizationId);

        Task<IResult<RoleDto>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, Guid organizationId);

        Task<IResult<bool>> DeleteRoleAsync(Guid roleId, Guid organizationId);

        Task<IResult<List<backend.Models.DTOs.Permission.PermissionDto>>> GetRolePermissionsAsync(Guid roleId);

        Task<IResult<bool>> AssignPermissionsToRoleAsync(Guid roleId, UpdateRolePermissionsRequest request, Guid organizationId);

        Task<IResult<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid organizationId);

        Task<IResult<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId, Guid organizationId);

        Task<IResult<List<RoleDto>>> GetUserRolesAsync(Guid userId, Guid organizationId);
    }
}