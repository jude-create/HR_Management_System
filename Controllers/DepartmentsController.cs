using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// DepartmentsController handles department list, detail, create, update, and delete endpoints.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // Returns all departments with member counts.
    [HttpGet]
    public ActionResult<IReadOnlyList<DepartmentDto>> GetDepartments()
        => Ok(_departmentService.GetDepartments());

    // Returns one department and its members.
    [HttpGet("{id:guid}")]
    public ActionResult<DepartmentDetailDto> GetDepartment(Guid id)
    {
        var department = _departmentService.GetDepartment(id);
        return department is null ? NotFound() : Ok(department);
    }

    // Creates a new department.
    [HttpPost]
    public ActionResult<DepartmentDto> CreateDepartment([FromBody] DepartmentUpsertRequest request)
    {
        var result = _departmentService.CreateDepartment(request);

        return result.Error switch
        {
            DepartmentOperationError.None => Ok(result.Department),
            DepartmentOperationError.DuplicateName => Conflict("A department with this name already exists."),
            _ => BadRequest("Invalid department payload.")
        };
    }

    // Updates an existing department.
    [HttpPut("{id:guid}")]
    public ActionResult<DepartmentDto> UpdateDepartment(Guid id, [FromBody] DepartmentUpsertRequest request)
    {
        var result = _departmentService.UpdateDepartment(id, request);

        return result.Error switch
        {
            DepartmentOperationError.None => Ok(result.Department),
            DepartmentOperationError.NotFound => NotFound("Department not found."),
            DepartmentOperationError.DuplicateName => Conflict("A department with this name already exists."),
            _ => BadRequest("Invalid department payload.")
        };
    }

    // Deletes a department only when it has no assigned employees.
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteDepartment(Guid id)
    {
        var result = _departmentService.DeleteDepartment(id);

        return result.Error switch
        {
            DepartmentOperationError.None => NoContent(),
            DepartmentOperationError.NotFound => NotFound("Department not found."),
            DepartmentOperationError.HasMembers => Conflict("This department still has employees assigned."),
            _ => BadRequest("Unable to delete department.")
        };
    }
}