namespace HrManagement.Api.Dtos.Employees;

// EmployeeDto is the safe response shape sent back to the client.
// It contains display-friendly values and leaves out internal fields like HiredAt.
public record EmployeeDto(
    Guid Id,
    string Name,
    string Title,
    string Email,
    string? AvatarUrl,
    string Type,           // enum sent as string over JSON, parsed back on the way in
    string Status,
    Guid DepartmentId,
    string DepartmentName
);

// This request is used for both create and update operations.
// The controller sends it to the service, which converts the text fields into the entity values.
public record EmployeeUpsertRequest(
    string Name,
    string Title,
    string Email,
    Guid DepartmentId,
    string Type,
    string Status,
    string? AvatarUrl
);
