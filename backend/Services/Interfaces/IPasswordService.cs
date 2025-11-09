using backend.Services.Common;
using backend.Models.DTOs.Auth;

namespace backend.Services.Interfaces
{
    public interface IPasswordService
    {
        Task<IResult<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);

        Task<IResult<bool>> InitiateForgotPasswordAsync(ForgotPasswordRequest request);

        Task<IResult<bool>> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult<bool>> ValidatePasswordStrengthAsync(string password);
    }
}