namespace HrManagement.Api.Dtos.Employees;

// Lightweight - for list views (e.g. a department dropdown or overview grid).
public record DepartmentDto(Guid Id, string Slug, string Name, int MemberCount);

// Heavier - only fetched when the user opens one specific department.
public record DepartmentDetailDto(Guid Id, string Slug, string Name, List<EmployeeDto> Members);

public record DepartmentUpsertRequest(string Name);
