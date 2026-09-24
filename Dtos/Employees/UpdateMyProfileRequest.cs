using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Employees;

public record UpdateMyProfileRequest(

    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Name,

    string? Mobile,

    DateTime? DateOfBirth,

    string? Gender,

    string? Nationality,

    string? Address,

    string? City,

    string? State,

    string? ZipCode,

    string? AvatarUrl,

    EmployeeLinksRequest? Links
);