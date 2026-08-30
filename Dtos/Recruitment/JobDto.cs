using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Recruitment;

public record JobDto(
    Guid Id,
    string Title,
    string Description,
    List<string> Roles,
    string Location,
    decimal SalaryMin,
    decimal SalaryMax,
    string Status,
    int CandidateCount
);

public record JobUpsertRequest(
    [ Required, StringLength(150, MinimumLength = 2)]
    string Title,

    [ Required, StringLength(2000)]
    string Description,

    [ Required]
    Guid DepartmentId,

    [ Required, StringLength(100)]
    string Location,

    [ Range(0, double.MaxValue, ErrorMessage = "SalaryMin cannot be negative.")]
    decimal SalaryMin,

    [ Range(0, double.MaxValue, ErrorMessage = "SalaryMax cannot be negative.")]
    decimal SalaryMax,

    [ MinLength(1, ErrorMessage = "At least one role is required.")]
    List<string> Roles
);

public record JobStatusRequest(
    [ Required]
    string Status
);