namespace HR_Management_System.Dtos.Employees;

public record EmployeeDto(
    Guid Id,

    string EmployeeNumber,

    string Name,

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

    string Title,

    string Type,

    string Status,

    DateTime JoiningDate,

    string? OfficeLocation,

    Guid DepartmentId,

    string DepartmentName,

    EmployeeLinksDto? Links,

    List<EmployeeDocumentDto> Documents
);