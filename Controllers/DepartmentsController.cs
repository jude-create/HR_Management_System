using HrManagement.Api.Dtos.Employees;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// DepartmentsController handles department list, detail, create, update, and delete endpoints.
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
        var department = _departmentService.CreateDepartment(request);
        return department is null ? BadRequest("Invalid department payload.") : Ok(department);
    }

    // Updates an existing department.
    [HttpPut("{id:guid}")]
    public ActionResult<DepartmentDto> UpdateDepartment(Guid id, [FromBody] DepartmentUpsertRequest request)
    {
        var department = _departmentService.UpdateDepartment(id, request);
        return department is null ? NotFound() : Ok(department);
    }

    // Deletes a department only when it has no assigned employees.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteDepartment(Guid id)
    {
        var deleted = _departmentService.DeleteDepartment(id);
        return deleted ? NoContent() : NotFound("Department not found, or it still has employees assigned.");
    }
}
