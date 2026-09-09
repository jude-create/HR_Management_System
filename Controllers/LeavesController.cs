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

    [HttpGet("employee/{employeeId:guid}")]
    public ActionResult<PagedResponse<LeaveDto>> GetEmployeeLeaves(
        Guid employeeId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(
            _leaveService.GetEmployeeLeaves(
                employeeId,
                page,
                pageSize
            )
        );
    }

    [HttpPost("employee/{employeeId:guid}")]
    public ActionResult<LeaveDto> CreateLeave(
        Guid employeeId,
        [FromBody] CreateLeaveRequest request)
    {
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

            LeaveOperationError.InvalidStatus =>
                BadRequest("Invalid leave status."),

            _ =>
                BadRequest("Unable to create leave.")
        };
    }

    [HttpPut("{id:guid}")]
    public ActionResult<LeaveDto> UpdateLeave(
        Guid id,
        [FromBody] UpdateLeaveRequest request)
    {
        var result = _leaveService.UpdateLeave(
            id,
            request
        );

        return result.Error switch
        {
            LeaveOperationError.None =>
                Ok(result.Leave),

            LeaveOperationError.NotFound =>
                NotFound("Leave record not found."),

            LeaveOperationError.InvalidDateRange =>
                BadRequest("End date cannot be before start date."),

            LeaveOperationError.InvalidStatus =>
                BadRequest("Invalid leave status."),

            _ =>
                BadRequest("Unable to update leave.")
        };
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteLeave(Guid id)
    {
        var result = _leaveService.DeleteLeave(id);

        return result.Error switch
        {
            LeaveOperationError.None =>
                NoContent(),

            LeaveOperationError.NotFound =>
                NotFound("Leave record not found."),

            _ =>
                BadRequest("Unable to delete leave.")
        };
    }
}