using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Employees;

public record EmployeeDocumentDto(
    Guid Id,
    string DocumentType,
    string FileName,
    string FileUrl,
    DateTime UploadedAt
);


