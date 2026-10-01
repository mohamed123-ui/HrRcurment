using SmartRecruitment.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRecruitment.Application.Contract
{
    public interface IAuthService
    {

        Task<LoginResponse>RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<LoginResponse>LoginAsync(LoginRequest loginRequest);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest);
        Task ResetPasswordAsync(ResetPasswordRequest request);

    }
}
