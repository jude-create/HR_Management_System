using HR_Management_System.Dtos.Employees;

namespace HR_Management_System.Dtos.Common;

public enum DepartmentOperationError
{
    None,
    NotFound,
    DuplicateName,
    HasMembers
}

public sealed record DepartmentResult(DepartmentDto? Department, DepartmentOperationError Error)
{
    public static DepartmentResult Success(DepartmentDto dto) => new(dto, DepartmentOperationError.None);
    public static DepartmentResult Fail(DepartmentOperationError error) => new(null, error);
}

public sealed record DeleteDepartmentResult(bool Success, DepartmentOperationError Error)
{
    public static DeleteDepartmentResult Ok() => new(true, DepartmentOperationError.None);
    public static DeleteDepartmentResult Fail(DepartmentOperationError error) => new(false, error);
}