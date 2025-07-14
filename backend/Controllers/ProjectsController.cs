// backend/Controllers/ProjectsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Models.DTOs.Project;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        // GET api/projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var organizationId = GetCurrentUserOrganizationId();
            
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "Invalid organization" });
            }

            var result = await _projectService.GetProjectsAsync(organizationId);
            return HandleServiceResult(result);
        }

        // GET api/projects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
        {
            var organizationId = GetCurrentUserOrganizationId();
            
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "Invalid organization" });
            }

            var result = await _projectService.GetProjectByIdAsync(id, organizationId);
            return HandleServiceResult(result);
        }

        // POST api/projects
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request)
        {
            var organizationId = GetCurrentUserOrganizationId();
            
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "Invalid organization" });
            }

            var result = await _projectService.CreateProjectAsync(request, organizationId);
            
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetProject), 
                    new { id = result.Data!.Id }, 
                    new { success = true, data = result.Data });
            }

            return HandleServiceResult(result);
        }

        // PUT api/projects/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var organizationId = GetCurrentUserOrganizationId();
            
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "Invalid organization" });
            }

            var result = await _projectService.UpdateProjectAsync(id, request, organizationId);
            return HandleServiceResult(result);
        }

        // DELETE api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(Guid id)
        {
            var organizationId = GetCurrentUserOrganizationId();
            
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "Invalid organization" });
            }

            var result = await _projectService.DeleteProjectAsync(id, organizationId);
            return HandleServiceResult(result, "Project deleted successfully");
        }

        // POST api/projects/{id}/assign-user
        [HttpPost("{id}/assign-user")]
        public async Task<ActionResult> AssignUserToProject(Guid id, [FromBody] AssignUserRequest request)
        {
            var organizationId = GetCurrentUserOrganizationId();
            
            if (organizationId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "Invalid organization" });
            }

            var result = await _projectService.AssignUserToProjectAsync(id, request.UserId, organizationId);
            return HandleServiceResult(result, "User assigned to project successfully");
        }
    }

    // Additional DTO for user assignment
    public class AssignUserRequest
    {
        public Guid UserId { get; set; }
    }
}