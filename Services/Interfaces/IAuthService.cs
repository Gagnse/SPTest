// backend/Services/Interfaces/IAuthService.cs
using backend.Services.Common;
using backend.Models.DTOs.Auth;
using backend.Models.DTOs.User;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IResult<LoginResponse>> LoginAsync(LoginRequest request);
        Task<IResult<UserDto>> GetCurrentUserAsync(Guid userId);
        Task<IResult<bool>> LogoutAsync(Guid userId);
        Task<IResult<UserDto>> RegisterAsync(RegisterRequest request);
    }
}