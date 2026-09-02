using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR_Management_System.Entities;
using Microsoft.IdentityModel.Tokens;

namespace HR_Management_System.Services;

// Builds real, signed JWTs carrying the user's id, name, role, and permissions as claims.
public interface IJwtTokenService
{
    string CreateAccessToken(User user, out DateTime expiresAt);
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateAccessToken(User user, out DateTime expiresAt)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key was not found in configuration.");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var minutes) ? minutes : 120;

        expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        // Permissions go in as their own claim type so services/controllers can check them individually.
        claims.AddRange(user.Permissions.Select(p => new Claim("permission", p)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}