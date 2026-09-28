using System.ComponentModel.DataAnnotations;
using HR_Management_System.Entities.Enums;

namespace HR_Management_System.Dtos.Employees;

public record UpdateEmployeeRequest(

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
    EmployeeType Type,

    [Required]
    Guid DepartmentId,

    [Required]
    EmployeeStatus Status,

    DateTime? JoiningDate,

    string? OfficeLocation,

    EmployeeLinksRequest? Links
);




