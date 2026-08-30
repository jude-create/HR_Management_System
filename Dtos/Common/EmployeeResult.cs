using HR_Management_System.Dtos.Employees;

namespace HR_Management_System.Dtos.Common;

// Lets a service report a specific failure reason instead of a bare null,
// so the controller can map each case to the right HTTP status code.
public enum EmployeeOperationError
{
    None,
    NotFound,
    InvalidDepartment,
    InvalidType,
    InvalidStatus,
    HasDependencies // used for delete: employee still has payroll/attendance records
}

public sealed record EmployeeResult(EmployeeDto? Employee, EmployeeOperationError Error)
{
    public static EmployeeResult Success(EmployeeDto dto) => new(dto, EmployeeOperationError.None);
    public static EmployeeResult Fail(EmployeeOperationError error) => new(null, error);
}

// Delete doesn't return an EmployeeDto, so it gets its own lightweight result.
public sealed record DeleteResult(bool Success, EmployeeOperationError Error)
{
    public static DeleteResult Ok() => new(true, EmployeeOperationError.None);
    public static DeleteResult Fail(EmployeeOperationError error) => new(false, error);
}