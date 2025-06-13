// backend/Controllers/ProjectsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class ProjectsController : ControllerBase
    {
        private readonly AdminDbContext _context;

        public ProjectsController(AdminDbContext context)
        {
            _context = context;
        }

        // GET api/projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            try
            {
                var userId = GetCurrentUserId();
                var organizationId = GetCurrentUserOrganizationId();

                if (userId == Guid.Empty || organizationId == Guid.Empty)
                {
                    return Unauthorized();
                }

                // Get projects for the user's organization
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
                    .ToListAsync();

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des projets." });
            }
        }

        // GET api/projects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
        {
            try
            {
                var organizationId = GetCurrentUserOrganizationId();

                var project = await _context.Projects
                    .Where(p => p.Id == id && p.OrganizationId == organizationId)
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
                    return NotFound();
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du projet." });
            }
        }

        // POST api/projects
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request)
        {
            try
            {
                var organizationId = GetCurrentUserOrganizationId();

                if (organizationId == Guid.Empty)
                {
                    return Unauthorized();
                }

                // Check if project number already exists
                var existingProject = await _context.Projects
                    .FirstOrDefaultAsync(p => p.ProjectNumber == request.ProjectNumber && p.OrganizationId == organizationId);

                if (existingProject != null)
                {
                    return BadRequest(new { message = "Le numéro de projet existe déjà." });
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

                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création du projet." });
            }
        }

        // Helper methods
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private Guid GetCurrentUserOrganizationId()
        {
            var orgIdClaim = User.FindFirst("OrganizationId")?.Value;
            return Guid.TryParse(orgIdClaim, out var orgId) ? orgId : Guid.Empty;
        }
    }

    // DTOs
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "active";
        public string? Type { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? OrganizationName { get; set; }
    }

    public class CreateProjectRequest
    {
        public string ProjectNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Type { get; set; }
    }
}