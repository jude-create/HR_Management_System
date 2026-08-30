using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// EmployeesController handles employee CRUD and paging.
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // Returns a paged employee list for table/grid views.
    [HttpGet]
    public ActionResult<PagedResponse<EmployeeDto>> GetEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(_employeeService.GetEmployees(page, pageSize));

    // Returns one employee by ID.
    [HttpGet("{id:guid}")]
    public ActionResult<EmployeeDto> GetEmployee(Guid id)
    {
        var employee = _employeeService.GetEmployee(id);
        return employee is null ? NotFound() : Ok(employee);
    }

    // Creates a new employee record.
    [HttpPost]
    public ActionResult<EmployeeDto> CreateEmployee([FromBody] EmployeeUpsertRequest request)
    {
        var result = _employeeService.CreateEmployee(request);

        return result.Error switch
        {
            EmployeeOperationError.None => Ok(result.Employee),
            EmployeeOperationError.InvalidDepartment => BadRequest("Department does not exist."),
            EmployeeOperationError.InvalidType => BadRequest("Invalid employee type."),
            EmployeeOperationError.InvalidStatus => BadRequest("Invalid employee status."),
            _ => BadRequest("Invalid employee payload.")
        };
    }

    // Updates an existing employee record.
    [HttpPut("{id:guid}")]
    public ActionResult<EmployeeDto> UpdateEmployee(Guid id, [FromBody] EmployeeUpsertRequest request)
    {
        var result = _employeeService.UpdateEmployee(id, request);

        return result.Error switch
        {
            EmployeeOperationError.None => Ok(result.Employee),
            EmployeeOperationError.NotFound => NotFound("Employee not found."),
            EmployeeOperationError.InvalidDepartment => BadRequest("Department does not exist."),
            EmployeeOperationError.InvalidType => BadRequest("Invalid employee type."),
            EmployeeOperationError.InvalidStatus => BadRequest("Invalid employee status."),
            _ => BadRequest("Invalid employee payload.")
        };
    }

    // Deletes an employee if no dependent records block it.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployee(Guid id)
    {
        var result = _employeeService.DeleteEmployee(id);

        return result.Error switch
        {
            EmployeeOperationError.None => NoContent(),
            EmployeeOperationError.NotFound => NotFound("Employee not found."),
            EmployeeOperationError.HasDependencies => Conflict("Linked payroll/attendance records still exist."),
            _ => BadRequest("Unable to delete employee.")
        };
    }
}