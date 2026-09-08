using HR_Management_System.Data;
using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Employees;

public record CreateEmployeeDocumentRequest(

    [Required]
    string DocumentType,

    [Required]
    string FileName,

    [Required]
    string FileUrl
);


