using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Leaves;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeavesController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeavesController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    private bool CanAccessEmployee(Guid employeeId)
    {
        if (User.IsInRole("Admin") ||
            User.IsInRole("HrManager"))
        {
            return true;
        }

        var employeeIdClaim =
            User.FindFirst("employeeId")?.Value;

        return Guid.TryParse(
            employeeIdClaim,
            out var loggedInEmployeeId)
            && loggedInEmployeeId == employeeId;
    }

    // GET: api/leaves
    [Authorize(Roles = "Admin,HrManager")]
    [HttpGet]
    public ActionResult<PagedResponse<LeaveDto>> GetAllLeaves(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        return Ok(
            _leaveService.GetAllLeaves(
                page,
                pageSize
            )
        );
    }

    // GET: api/leaves/employee/{employeeId}
    [HttpGet("employee/{employeeId:guid}")]
    public ActionResult<PagedResponse<LeaveDto>> GetEmployeeLeaves(
    Guid employeeId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        if (!CanAccessEmployee(employeeId))
        {
            return Forbid();
        }

        return Ok(
            _leaveService.GetEmployeeLeaves(
                employeeId,
                page,
                pageSize
            )
        );
    }

    // GET: api/leaves/my
    [HttpGet("my")]
    public ActionResult<PagedResponse<LeaveDto>> GetMyLeaves(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        var employeeIdClaim =
            User.FindFirst("employeeId")?.Value;

        if (!Guid.TryParse(
            employeeIdClaim,
            out var employeeId))
        {
            return Forbid();
        }

        return Ok(
            _leaveService.GetMyLeaves(
                employeeId,
                page,
                pageSize
            )
        );
    }

    // POST: api/leaves/employee/{employeeId}
    [HttpPost]
    public ActionResult<LeaveDto> CreateLeave(
    [FromBody] CreateLeaveRequest request)
    {
        var employeeIdClaim =
            User.FindFirst("employeeId")?.Value;

        if (!Guid.TryParse(
            employeeIdClaim,
            out var employeeId))
        {
            return Forbid();
        }

        var result = _leaveService.CreateLeave(
            employeeId,
            request
        );

        return result.Error switch
        {
            LeaveOperationError.None =>
                Ok(result.Leave),

            LeaveOperationError.NotFound =>
                NotFound("Employee not found."),

            LeaveOperationError.InvalidDateRange =>
                BadRequest("End date cannot be before start date."),

            LeaveOperationError.InvalidDays =>
                BadRequest("Leave duration must be at least one day."),

            LeaveOperationError.InsufficientLeaveBalance =>
BadRequest("Employee does not have enough leave balance."),

            LeaveOperationError.InvalidStatus =>
                BadRequest("Invalid leave status."),

            LeaveOperationError.InvalidLeaveType => BadRequest("Invalid leave type."),

            LeaveOperationError.CrossYearLeave =>
    BadRequest("Leave cannot cross calendar years."),

            _ =>
                BadRequest("Unable to create leave.")
        };
    }


    // PUT: api/leaves/{id}
    [HttpPut("{id:guid}")]
    public ActionResult<LeaveDto> UpdateLeave(
    Guid id,
    [FromBody] UpdateLeaveRequest request)
    {
        var isAdminOrHr =
            User.IsInRole("Admin") ||
            User.IsInRole("HrManager");

        Guid? loggedInEmployeeId = null;

        if (!isAdminOrHr)
        {
            var employeeIdClaim =
                User.FindFirst("employeeId")?.Value;

            if (!Guid.TryParse(
                employeeIdClaim,
                out var employeeId))
            {
                return Forbid();
            }

            loggedInEmployeeId = employeeId;
        }

        var result = _leaveService.UpdateLeave(
            id,
            request,
            loggedInEmployeeId
        );

        return result.Error switch
        {
            LeaveOperationError.None =>
                Ok(result.Leave),

            LeaveOperationError.NotFound =>
                NotFound("Leave record not found."),

            LeaveOperationError.Unauthorized =>
                Forbid(),

            LeaveOperationError.InvalidDateRange =>
                BadRequest("End date cannot be before start date."),

            LeaveOperationError.InvalidStatus =>
                BadRequest("Invalid leave status."),

            LeaveOperationError.InvalidLeaveType => BadRequest("Invalid leave type."),

            LeaveOperationError.CrossYearLeave =>
    BadRequest("Leave cannot cross calendar years."),

            _ =>
                BadRequest("Unable to update leave.")
        };
    }

    // PATCH: api/leaves/{id}/status
    [Authorize(Roles = "Admin,HrManager")]
    [HttpPatch("{id:guid}/status")]
    public ActionResult<LeaveDto> UpdateLeaveStatus(
    Guid id,
    [FromBody] UpdateLeaveStatusRequest request)
    {
        var result = _leaveService.UpdateLeaveStatus(
            id,
            request
        );

        return result.Error switch
        {
            LeaveOperationError.None =>
                Ok(result.Leave),

            LeaveOperationError.NotFound =>
                NotFound("Leave record not found."),

            LeaveOperationError.InvalidStatus =>
                BadRequest("Only Pending leave requests can be approved or rejected."),

            LeaveOperationError.InsufficientLeaveBalance =>
BadRequest("Employee does not have enough leave balance."),

            _ =>
                BadRequest("Unable to update leave status.")
        };
    }

    // DELETE: api/leaves/{id}
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteLeave(Guid id)
    {
        var isAdminOrHr =
            User.IsInRole("Admin") ||
            User.IsInRole("HrManager");

        Guid? loggedInEmployeeId = null;

        if (!isAdminOrHr)
        {
            var employeeIdClaim =
                User.FindFirst("employeeId")?.Value;

            if (!Guid.TryParse(
                employeeIdClaim,
                out var employeeId))
            {
                return Forbid();
            }

            loggedInEmployeeId = employeeId;
        }

        var result = _leaveService.DeleteLeave(
            id,
            loggedInEmployeeId
        );

        return result.Error switch
        {
            LeaveOperationError.None =>
                NoContent(),

            LeaveOperationError.NotFound =>
                NotFound("Leave record not found."),

            LeaveOperationError.Unauthorized =>
                Forbid(),

            _ =>
                BadRequest("Unable to delete leave.")
        };
    }
}