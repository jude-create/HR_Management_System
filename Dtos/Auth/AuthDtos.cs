namespace HrManagement.Api.Dtos.Auth;

// ---- requests (never persisted, just transport) ----
public record LoginRequest(string Email, string Password);
public record ForgotPasswordRequest(string Email);
public record VerifyOtpRequest(string Email, string Code);
public record UpdateProfileRequest(string? Name, string? Email);
public record UpdatePasswordRequest(string CurrentPassword, string NewPassword);

// Role/permission changes are an admin action on a *different* user,
// not something a person sets on their own profile - keep it a separate endpoint/DTO.
public record UpdateUserRoleRequest(string Role, List<string> Permissions);

// ---- responses ----
public record AuthUserDto(Guid Id, string Name, string Email, string Role, List<string> Permissions);
public record AuthSessionDto(string AccessToken, string RefreshToken, DateTime ExpiresAt, AuthUserDto User);
