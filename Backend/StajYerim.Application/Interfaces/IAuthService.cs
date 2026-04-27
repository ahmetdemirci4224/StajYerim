using StajYerim.Application.DTOs.Auth;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default);
        Task<Result<AuthResponseDto>> RegisterCompanyAsync(RegisterCompanyDto dto, CancellationToken cancellationToken = default);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default);
    }
}
