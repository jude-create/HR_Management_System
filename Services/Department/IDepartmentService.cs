using HR_Management_System.Dtos.Employees;

namespace HR_Management_System.Services.Department
{
    public interface IDepartmentService
    {
        IReadOnlyList<DepartmentDto> GetDepartments();
        DepartmentDetailDto? GetDepartment(string slug);
        DepartmentResult CreateDepartment(DepartmentUpsertRequest request);
        DepartmentResult UpdateDepartment(Guid id, DepartmentUpsertRequest request);
        DeleteDepartmentResult DeleteDepartment(Guid id);
    }
}
