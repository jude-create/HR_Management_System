namespace HrManagement.Api.Dtos.Recruitment;

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
    string Title,
    string Description,
    Guid DepartmentId,
    string Location,
    decimal SalaryMin,
    decimal SalaryMax,
    List<string> Roles
);

public record JobStatusRequest(string Status);
