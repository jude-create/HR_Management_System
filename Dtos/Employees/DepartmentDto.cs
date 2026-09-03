using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Employees;

public record DepartmentDto(
    Guid Id,
    string Slug,
    string Name,
    int MemberCount,
    List<EmployeeDto> Members
);

public record DepartmentDetailDto(Guid Id, string Slug, string Name, List<EmployeeDto> Members);

public record DepartmentUpsertRequest(
    [ Required, StringLength(100, MinimumLength = 2)]
    string Name
);