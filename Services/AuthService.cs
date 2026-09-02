using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Auth;
using HR_Management_System.Entities;
using HR_Management_System.Dtos.Common;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

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

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(AppDbContext context, IMapper mapper, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _mapper = mapper;
        _jwtTokenService = jwtTokenService;
    }

    public AuthSessionDto? Login(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var user = _context.Users
            .Include(x => x.Settings)
            .FirstOrDefault(x => x.Email.ToLower() == normalizedEmail);

        if (user is null || user.PasswordHash != HrServiceSupport.HashPassword(request.Password))
        {
            return null;
        }

        var accessToken = _jwtTokenService.CreateAccessToken(user, out var expiresAt);

        return new AuthSessionDto(
            accessToken,
            HrServiceSupport.CreateToken(),
            expiresAt,
            _mapper.Map<AuthUserDto>(user)
        );
    }

    public ApiMessageResponse ForgotPassword(ForgotPasswordRequest request)
        => new($"Password reset instructions were queued for {request.Email}.");

    public ApiMessageResponse VerifyOtp(VerifyOtpRequest request)
        => new($"OTP verified for {request.Email}.");

    public AuthUserDto? UpdateProfile(UpdateProfileRequest request)
    {
        var user = _context.Users
            .Include(x => x.Settings)
            .FirstOrDefault(x => x.Id == _context.CurrentUserId);
        if (user is null)
        {
            return null;
        }

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
        var user = _context.Users.FirstOrDefault(x => x.Id == _context.CurrentUserId);
        if (user is null || user.PasswordHash != HrServiceSupport.HashPassword(request.CurrentPassword))
        {
            return null;
        }

        user.PasswordHash = HrServiceSupport.HashPassword(request.NewPassword);
        _context.SaveChanges();
        return new ApiMessageResponse("Password updated successfully.");
    }

    public AuthUserDto? UpdateUserRole(Guid userId, UpdateUserRoleRequest request)
    {
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

    public IReadOnlyList<AuthUserDto> GetUsers()
    {
        var users = _context.Users.OrderBy(x => x.Name).ToList();
        return _mapper.Map<List<AuthUserDto>>(users);
    }
}