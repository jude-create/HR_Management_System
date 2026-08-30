using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Auth;
using HR_Management_System.Entities;
using HR_Management_System.Dtos.Common;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// AuthService owns login and user-account-related logic.
// It does not know anything about HTTP routes; it only works with request models
// and returns values or null when the action cannot be completed.
public interface IAuthService
{
    AuthSessionDto? Login(LoginRequest request);
    ApiMessageResponse ForgotPassword(ForgotPasswordRequest request);
    ApiMessageResponse VerifyOtp(VerifyOtpRequest request);
    AuthUserDto? UpdateProfile(UpdateProfileRequest request);
    ApiMessageResponse? UpdatePassword(UpdatePasswordRequest request);
    AuthUserDto? UpdateUserRole(Guid userId, UpdateUserRoleRequest request);
}

// This service works on the shared in-memory store and behaves like a lightweight
// authentication layer for testing and learning.
public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AuthService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public AuthSessionDto? Login(LoginRequest request)
    {
        // Step 1: find the account by email.
        // Step 2: hash the supplied password.
        // Step 3: compare the hashes. If they match, create a session response.
        var user = _context.Users
            .Include(x => x.Settings)
            .FirstOrDefault(x => x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (user is null || user.PasswordHash != HrServiceSupport.HashPassword(request.Password))
        {
            return null;
        }

        // On success we return a session object with tokens and the user profile.
        return new AuthSessionDto(
            HrServiceSupport.CreateToken(),
            HrServiceSupport.CreateToken(),
            DateTime.UtcNow.AddDays(7),
            _mapper.Map<AuthUserDto>(user)
        );
    }

    public ApiMessageResponse ForgotPassword(ForgotPasswordRequest request)
        // Demo response only.
        // A real system would create a reset token and send an email.
        => new($"Password reset instructions were queued for {request.Email}.");

    public ApiMessageResponse VerifyOtp(VerifyOtpRequest request)
        // Demo response only.
        // In production this should compare the submitted code with a stored OTP.
        => new($"OTP verified for {request.Email}.");

    public AuthUserDto? UpdateProfile(UpdateProfileRequest request)
    {
        // We use CurrentUserId as our demo "signed in user" because we do not have
        // real auth middleware hooked up yet.
        var user = _context.Users
            .Include(x => x.Settings)
            .FirstOrDefault(x => x.Id == _context.CurrentUserId);
        if (user is null)
        {
            return null;
        }

        // Only overwrite values that were actually provided in the request body.
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.Email = request.Email.Trim();
        }

        _context.SaveChanges();
        return _mapper.Map<AuthUserDto>(user);
    }

    public ApiMessageResponse? UpdatePassword(UpdatePasswordRequest request)
    {
        // We require the current password before allowing a new one.
        var user = _context.Users.FirstOrDefault(x => x.Id == _context.CurrentUserId);
        if (user is null || user.PasswordHash != HrServiceSupport.HashPassword(request.CurrentPassword))
        {
            return null;
        }

        // Store only the hashed version. Never keep raw passwords in memory or in a DB.
        user.PasswordHash = HrServiceSupport.HashPassword(request.NewPassword);
        _context.SaveChanges();
        return new ApiMessageResponse("Password updated successfully.");
    }

    public AuthUserDto? UpdateUserRole(Guid userId, UpdateUserRoleRequest request)
    {
        // This is an admin-style action: one user is changing another user's role.
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user is null || !Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            return null;
        }

        user.Role = role;
        user.Permissions = request.Permissions.ToList();
        _context.SaveChanges();
        return _mapper.Map<AuthUserDto>(user);
    }
}
