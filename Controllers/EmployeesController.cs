using HrManagement.Api.Dtos.Employees;
using HrManagement.Api.Dtos.Common;
using HrManagement.Api.Services;
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
        var employee = _employeeService.CreateEmployee(request);
        return employee is null ? BadRequest("Invalid employee payload.") : Ok(employee);
    }

    // Updates an existing employee record.
    [HttpPut("{id:guid}")]
    public ActionResult<EmployeeDto> UpdateEmployee(Guid id, [FromBody] EmployeeUpsertRequest request)
    {
        var employee = _employeeService.UpdateEmployee(id, request);
        return employee is null ? NotFound() : Ok(employee);
    }

    // Deletes an employee if no dependent records block it.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployee(Guid id)
    {
        var deleted = _employeeService.DeleteEmployee(id);
        return deleted ? NoContent() : NotFound("Employee not found, or linked payroll/attendance records still exist.");
    }
}
