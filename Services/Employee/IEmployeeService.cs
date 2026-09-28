using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Employees;

namespace HR_Management_System.Services.Employee
{
    public interface IEmployeeService
    {
        PagedResponse<EmployeeDto> GetEmployees(
            int page,
            int pageSize);

        EmployeeDto? GetEmployee(Guid id);

        EmployeeResult CreateEmployee(
            CreateEmployeeRequest request);

        EmployeeResult UpdateEmployee(
            Guid id,
            UpdateEmployeeRequest request);

        EmployeeResult UpdateMyProfile(
        Guid employeeId,
        UpdateMyProfileRequest request
    );
        DeleteResult DeleteEmployee(Guid id);
    }

}
