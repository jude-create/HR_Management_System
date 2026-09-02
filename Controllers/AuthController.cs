using HR_Management_System.Dtos.Auth;
using HR_Management_System.Services;
using HR_Management_System.Dtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Open to everyone — this is how a user gets a token in the first place.
    [HttpPost("login")]
    public ActionResult<AuthSessionDto> Login([FromBody] LoginRequest request)
    {
        var session = _authService.Login(request);
        return session is null ? Unauthorized() : Ok(session);
    }

    [HttpPost("forgot-password")]
    public ActionResult<ApiMessageResponse> ForgotPassword([FromBody] ForgotPasswordRequest request)
        => Ok(_authService.ForgotPassword(request));

    [HttpPost("verify-otp")]
    public ActionResult<ApiMessageResponse> VerifyOtp([FromBody] VerifyOtpRequest request)
        => Ok(_authService.VerifyOtp(request));

    // Requires login — a user can only update their own profile.
    [Authorize]
    [HttpPut("profile")]
    public ActionResult<AuthUserDto> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var user = _authService.UpdateProfile(request);
        return user is null ? NotFound() : Ok(user);
    }

    // Requires login — a user can only change their own password.
    [Authorize]
    [HttpPut("password")]
    public ActionResult<ApiMessageResponse> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        var result = _authService.UpdatePassword(request);
        return result is null ? Unauthorized() : Ok(result);
    }

    // Admin-only — changing someone else's role/permissions is a privileged action.
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{userId:guid}/role")]
    public ActionResult<AuthUserDto> UpdateUserRole(Guid userId, [FromBody] UpdateUserRoleRequest request)
    {
        var user = _authService.UpdateUserRole(userId, request);
        return user is null ? NotFound() : Ok(user);
    }

    // Admin-only — lists every login-capable account, including sensitive role/permission data.
    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public ActionResult<IReadOnlyList<AuthUserDto>> GetUsers()
        => Ok(_authService.GetUsers());
}