namespace HR_Management_System.Entities;

// One row per login-capable account. This is what LoginRequest/AuthUserDto map to.
public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public List<string> Permissions { get; set; } = new();

    // Employee account relationship.
    // Admin and HR users will have this as null.
    // Employee users will point to their Employee record.
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public UserSettings Settings { get; set; } = null!;
}