using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public ActionResult<PagedResponse<EmployeeDto>> GetEmployees(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(
            _employeeService.GetEmployees(
                page,
                pageSize));
    }

    // GET: api/employees/{id}
    [HttpGet("{id:guid}")]
    public ActionResult<EmployeeDto> GetEmployee(Guid id)
    {
        var employee =
            _employeeService.GetEmployee(id);

        if (employee is null)
        {
            return NotFound(
                new
                {
                    message = "Employee not found."
                });
        }

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public ActionResult<EmployeeDto> CreateEmployee(
        [FromBody] CreateEmployeeRequest request)
    {
        var result =
            _employeeService.CreateEmployee(request);

        return result.Error switch
        {
            EmployeeOperationError.None =>
                Ok(result.Employee),

            EmployeeOperationError.InvalidDepartment =>
                BadRequest(
                    new
                    {
                        message =
                            "Department does not exist."
                    }),

            EmployeeOperationError.InvalidType =>
                BadRequest(
                    new
                    {
                        message =
                            "Invalid employee type."
                    }),

            EmployeeOperationError.InvalidStatus =>
                BadRequest(
                    new
                    {
                        message =
                            "Invalid employee status."
                    }),

            EmployeeOperationError.DuplicateEmail =>
                Conflict(
                    new
                    {
                        message =
                            "An employee with this email already exists."
                    }),

            _ =>
                BadRequest(
                    new
                    {
                        message =
                            "Invalid employee payload."
                    })
        };
    }

    // PUT: api/employees/{id}
    [HttpPut("{id:guid}")]
    public ActionResult<EmployeeDto> UpdateEmployee(
        Guid id,
        [FromBody] UpdateEmployeeRequest request)
    {
        var result =
            _employeeService.UpdateEmployee(
                id,
                request);

        return result.Error switch
        {
            EmployeeOperationError.None =>
                Ok(result.Employee),

            EmployeeOperationError.NotFound =>
                NotFound(
                    new
                    {
                        message =
                            "Employee not found."
                    }),

            EmployeeOperationError.InvalidDepartment =>
                BadRequest(
                    new
                    {
                        message =
                            "Department does not exist."
                    }),

            EmployeeOperationError.InvalidType =>
                BadRequest(
                    new
                    {
                        message =
                            "Invalid employee type."
                    }),

            EmployeeOperationError.InvalidStatus =>
                BadRequest(
                    new
                    {
                        message =
                            "Invalid employee status."
                    }),

            EmployeeOperationError.DuplicateEmail =>
                Conflict(
                    new
                    {
                        message =
                            "An employee with this email already exists."
                    }),

            _ =>
                BadRequest(
                    new
                    {
                        message =
                            "Invalid employee payload."
                    })
        };
    }

    // DELETE: api/employees/{id}
    [Authorize(Roles = "Admin,HrManager")]
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployee(Guid id)
    {
        var result =
            _employeeService.DeleteEmployee(id);

        return result.Error switch
        {
            EmployeeOperationError.None =>
                NoContent(),

            EmployeeOperationError.NotFound =>
                NotFound(
                    new
                    {
                        message =
                            "Employee not found."
                    }),

            EmployeeOperationError.HasDependencies =>
                Conflict(
                    new
                    {
                        message =
                            "Linked payroll or attendance records still exist."
                    }),

            _ =>
                BadRequest(
                    new
                    {
                        message =
                            "Unable to delete employee."
                    })
        };
    }
}