using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Employees;

public record CreateEmployeeRequest(

    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Name,

    [Required]
    [EmailAddress]
    string Email,

    string? Mobile,

    DateTime? DateOfBirth,

    string? Gender,

    string? Nationality,

    string? Address,

    string? City,

    string? State,

    string? ZipCode,

    string? AvatarUrl,

    [Required]
    [StringLength(100)]
    string Title,

    [Required]
    string Type,

    [Required]
    Guid DepartmentId,

    [Required]
    string Status,

    DateTime? JoiningDate,

    string? OfficeLocation,

    EmployeeLinksRequest? Links
);



