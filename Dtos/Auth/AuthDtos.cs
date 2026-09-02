using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Auth;

// ---- requests (never persisted, just transport) ----
public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record ForgotPasswordRequest([Required, EmailAddress] string Email);

public record VerifyOtpRequest(
    [Required, EmailAddress] string Email,
    [Required] string Code
);

public record UpdateProfileRequest(string? Name, [EmailAddress] string? Email);

public record UpdatePasswordRequest(
    [Required] string CurrentPassword,
    [Required, MinLength(8, ErrorMessage = "New password must be at least 8 characters.")] string NewPassword
);

// Role/permission changes are an admin action on a *different* user,
// not something a person sets on their own profile - keep it a separate endpoint/DTO.
public record UpdateUserRoleRequest(
    [Required] string Role,
    List<string> Permissions
);

// ---- responses ----
public record AuthUserDto(Guid Id, string Name, string Email, string Role, List<string> Permissions);
public record AuthSessionDto(string AccessToken, string RefreshToken, DateTime ExpiresAt, AuthUserDto User);