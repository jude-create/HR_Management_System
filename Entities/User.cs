namespace HR_Management_System.Entities;

// One row per login-capable account. This is what LoginRequest/AuthUserDto map to.
public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; } 
    public required string PasswordHash { get; set; }  // never expose this in a DTO
    public UserRole Role { get; set; }
    public List<string> Permissions { get; set; } = new();

    // 1-to-1: every user has exactly one settings row, created alongside the user.
    public UserSettings Settings { get; set; } = null!;
}
