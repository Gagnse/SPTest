// backend/Services/ProjectService.cs
using backend.Services.Common;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;
using backend.Models.DTOs.Project;


namespace backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AdminDbContext _context;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(AdminDbContext context, ILogger<ProjectService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IResult<IEnumerable<ProjectDto>>> GetProjectsAsync(Guid organizationId)
        {
            try
            {
                _logger.LogInformation("Getting projects for organization {OrganizationId}", organizationId);

                var projects = await _context.Projects
                    .Where(p => p.OrganizationId == organizationId)
                    .Include(p => p.Organization)
                    .Select(p => new ProjectDto
                    {
                        Id = p.Id,
                        ProjectNumber = p.ProjectNumber,
                        Name = p.Name,
                        Description = p.Description,
                        Status = p.Status ?? "active",
                        Type = p.Type,
                        ImageUrl = p.ImageUrl,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        OrganizationName = p.Organization!.Name
                    })
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                _logger.LogInformation("Found {ProjectCount} projects for organization {OrganizationId}", 
                    projects.Count, organizationId);

                return Result<IEnumerable<ProjectDto>>.Success(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects for organization {OrganizationId}", organizationId);
                return Result<IEnumerable<ProjectDto>>.Failure("Error retrieving projects");
            }
        }

        public async Task<IResult<ProjectDto>> GetProjectByIdAsync(Guid projectId, Guid organizationId)
        {
            try
            {
                var project = await _context.Projects
                    .Where(p => p.Id == projectId && p.OrganizationId == organizationId)
                    .Include(p => p.Organization)
                    .Select(p => new ProjectDto
                    {
                        Id = p.Id,
                        ProjectNumber = p.ProjectNumber,
                        Name = p.Name,
                        Description = p.Description,
                        Status = p.Status ?? "active",
                        Type = p.Type,
                        ImageUrl = p.ImageUrl,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        OrganizationName = p.Organization!.Name
                    })
                    .FirstOrDefaultAsync();

                if (project == null)
                {
                    return Result<ProjectDto>.Failure("Project not found");
                }

                return Result<ProjectDto>.Success(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project {ProjectId} for organization {OrganizationId}", 
                    projectId, organizationId);
                return Result<ProjectDto>.Failure("Error retrieving project");
            }
        }

        public async Task<IResult<ProjectDto>> CreateProjectAsync(CreateProjectRequest request, Guid organizationId)
        {
            try
            {
                // Validate request
                var validationResult = ValidateCreateProjectRequest(request);
                if (!validationResult.IsValid)
                {
                    return Result<ProjectDto>.Failure(validationResult.Errors);
                }

                // Check if project number already exists
                var existingProject = await _context.Projects
                    .FirstOrDefaultAsync(p => p.ProjectNumber == request.ProjectNumber && 
                                           p.OrganizationId == organizationId);

                if (existingProject != null)
                {
                    return Result<ProjectDto>.Failure("Project number already exists");
                }

                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    ProjectNumber = request.ProjectNumber,
                    Name = request.Name,
                    Description = request.Description,
                    Type = request.Type,
                    Status = "active",
                    StartDate = DateTime.UtcNow,
                    ImageUrl = "/static/images/project-placeholder.png"
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created project {ProjectId} for organization {OrganizationId}", 
                    project.Id, organizationId);

                var projectDto = new ProjectDto
                {
                    Id = project.Id,
                    ProjectNumber = project.ProjectNumber,
                    Name = project.Name,
                    Description = project.Description,
                    Status = project.Status,
                    Type = project.Type,
                    ImageUrl = project.ImageUrl,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate
                };

                return Result<ProjectDto>.Success(projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project for organization {OrganizationId}", organizationId);
                return Result<ProjectDto>.Failure("Error creating project");
            }
        }

        public async Task<IResult<ProjectDto>> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid organizationId)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == organizationId);

                if (project == null)
                {
                    return Result<ProjectDto>.Failure("Project not found");
                }

                // Update fields
                if (!string.IsNullOrEmpty(request.Name))
                    project.Name = request.Name;
                
                if (request.Description != null)
                    project.Description = request.Description;
                
                if (!string.IsNullOrEmpty(request.Type))
                    project.Type = request.Type;
                
                if (!string.IsNullOrEmpty(request.Status))
                    project.Status = request.Status;

                if (request.EndDate.HasValue)
                    project.EndDate = request.EndDate;

                await _context.SaveChangesAsync();

                var projectDto = new ProjectDto
                {
                    Id = project.Id,
                    ProjectNumber = project.ProjectNumber,
                    Name = project.Name,
                    Description = project.Description,
                    Status = project.Status ?? "active",
                    Type = project.Type,
                    ImageUrl = project.ImageUrl,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate
                };

                return Result<ProjectDto>.Success(projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId}", projectId);
                return Result<ProjectDto>.Failure("Error updating project");
            }
        }

        public async Task<IResult<bool>> DeleteProjectAsync(Guid projectId, Guid organizationId)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == organizationId);

                if (project == null)
                {
                    return Result<bool>.Failure("Project not found");
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted project {ProjectId}", projectId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId}", projectId);
                return Result<bool>.Failure("Error deleting project");
            }
        }

        public async Task<IResult<bool>> AssignUserToProjectAsync(Guid projectId, Guid userId, Guid organizationId)
        {
            try
            {
                // Verify project exists and belongs to organization
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == organizationId);

                if (project == null)
                {
                    return Result<bool>.Failure("Project not found");
                }

                // Verify user exists and belongs to organization
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.OrganizationId == organizationId);

                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                // Check if assignment already exists
                var existingAssignment = await _context.ProjectUsers
                    .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);

                if (existingAssignment != null)
                {
                    return Result<bool>.Failure("User already assigned to project");
                }

                // Create assignment
                var projectUser = new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                };

                _context.ProjectUsers.Add(projectUser);
                await _context.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user {UserId} to project {ProjectId}", userId, projectId);
                return Result<bool>.Failure("Error assigning user to project");
            }
        }

        private ValidationResult ValidateCreateProjectRequest(CreateProjectRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.ProjectNumber))
                errors.Add("Project number is required");

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Project name is required");

            if (request.Name?.Length > 200)
                errors.Add("Project name cannot exceed 200 characters");

            if (request.Description?.Length > 1000)
                errors.Add("Project description cannot exceed 1000 characters");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}