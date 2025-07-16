
// backend/Services/Interfaces/IProjectService.cs
using backend.Services.Common;
using backend.Models.DTOs.Project;

namespace backend.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IResult<IEnumerable<ProjectDto>>> GetProjectsAsync(Guid organizationId);
        Task<IResult<ProjectDto>> GetProjectByIdAsync(Guid projectId, Guid organizationId);
        Task<IResult<ProjectDto>> CreateProjectAsync(CreateProjectRequest request, Guid organizationId);
        Task<IResult<ProjectDto>> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid organizationId);
        Task<IResult<bool>> DeleteProjectAsync(Guid projectId, Guid organizationId);
        Task<IResult<bool>> AssignUserToProjectAsync(Guid projectId, Guid userId, Guid organizationId);
    }
}