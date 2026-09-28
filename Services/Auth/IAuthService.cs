using HR_Management_System.Dtos.Auth;
using HR_Management_System.Dtos.Common;

namespace HR_Management_System.Services.Auth
{
    public interface IAuthService
    {
        AuthSessionDto? Login(LoginRequest request);
        ApiMessageResponse ForgotPassword(ForgotPasswordRequest request);
        ApiMessageResponse VerifyOtp(VerifyOtpRequest request);
        AuthUserDto? UpdateProfile(UpdateProfileRequest request);
        ApiMessageResponse? UpdatePassword(UpdatePasswordRequest request);
        AuthUserDto? UpdateUserRole(Guid userId, UpdateUserRoleRequest request);
        IReadOnlyList<AuthUserDto> GetUsers(); // new
    }

}
