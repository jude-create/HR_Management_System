using HrManagement.Api.Dtos.Auth;
using HrManagement.Api.Dtos.Common;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// AuthController exposes login and account-management endpoints.
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Signs a user in and returns a token/session payload.
    [HttpPost("login")]
    public ActionResult<AuthSessionDto> Login([FromBody] LoginRequest request)
    {
        var session = _authService.Login(request);
        return session is null ? Unauthorized() : Ok(session);
    }

    // Demo-only endpoint that pretends to start the forgot-password flow.
    [HttpPost("forgot-password")]
    public ActionResult<ApiMessageResponse> ForgotPassword([FromBody] ForgotPasswordRequest request)
        => Ok(_authService.ForgotPassword(request));

    // Demo-only OTP verification endpoint.
    [HttpPost("verify-otp")]
    public ActionResult<ApiMessageResponse> VerifyOtp([FromBody] VerifyOtpRequest request)
        => Ok(_authService.VerifyOtp(request));

    // Updates the current user's profile fields.
    [HttpPut("profile")]
    public ActionResult<AuthUserDto> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var user = _authService.UpdateProfile(request);
        return user is null ? NotFound() : Ok(user);
    }

    // Changes the current user's password after confirming the old one.
    [HttpPut("password")]
    public ActionResult<ApiMessageResponse> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        var result = _authService.UpdatePassword(request);
        return result is null ? Unauthorized() : Ok(result);
    }

    // Admin endpoint that changes another user's role and permissions.
    [HttpPut("users/{userId:guid}/role")]
    public ActionResult<AuthUserDto> UpdateUserRole(Guid userId, [FromBody] UpdateUserRoleRequest request)
    {
        var user = _authService.UpdateUserRole(userId, request);
        return user is null ? NotFound() : Ok(user);
    }
}
