// backend/Controllers/BaseController.cs
using Microsoft.AspNetCore.Mvc;
using backend.Services.Common;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        protected Guid GetCurrentUserOrganizationId()
        {
            var orgIdClaim = User.FindFirst("OrganizationId")?.Value;
            return Guid.TryParse(orgIdClaim, out var orgId) ? orgId : Guid.Empty;
        }

        protected string? GetCurrentUserRole()
        {
            return User.FindFirst("Role")?.Value;
        }

        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        protected bool IsUserAuthenticated()
        {
            return GetCurrentUserId() != Guid.Empty;
        }

        // Helper method to handle service results consistently
        protected ActionResult HandleServiceResult<T>(IResult<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Data
                });
            }

            // Handle different types of errors
            if (result.ErrorMessage?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new
                {
                    success = false,
                    message = result.ErrorMessage
                });
            }

            if (result.ErrorMessage?.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = result.ErrorMessage
                });
            }

            if (result.Errors?.Any() == true)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = result.Errors
                });
            }

            return StatusCode(500, new
            {
                success = false,
                message = result.ErrorMessage ?? "An error occurred"
            });
        }

        // Overload for boolean results
        protected ActionResult HandleServiceResult(IResult<bool> result, string? successMessage = null)
        {
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = successMessage ?? "Operation completed successfully"
                });
            }

            return HandleServiceResult<bool>(result);
        }
    }
}