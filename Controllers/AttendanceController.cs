using HR_Management_System.Dtos.Attendance;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [Authorize(Roles = "Admin,HrManager")]
    [HttpGet]
    public ActionResult<PagedResponse<AttendanceDto>> GetAttendance(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] DateOnly? date = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        return Ok(
            _attendanceService.GetAttendance(
                page,
                pageSize,
                search,
                date,
                fromDate,
                toDate
            )
        );
    }

    [HttpGet("employee/{employeeId:guid}")]
    public ActionResult<PagedResponse<AttendanceDto>> GetEmployeeAttendance(
        Guid employeeId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var isAdminOrHr =
            User.IsInRole("Admin") ||
            User.IsInRole("HrManager");

        if (!isAdminOrHr)
        {
            var employeeIdClaim =
                User.FindFirst("employeeId")?.Value;

            if (!Guid.TryParse(
                    employeeIdClaim,
                    out var loggedInEmployeeId))
            {
                return Forbid();
            }

            if (loggedInEmployeeId != employeeId)
            {
                return Forbid();
            }
        }

        return Ok(
            _attendanceService.GetEmployeeAttendance(
                employeeId,
                page,
                pageSize,
                fromDate,
                toDate
            )
        );
    }



    // Marks an attendance record for correction review.
    [HttpPost("{id:guid}/correction")]
    public ActionResult<AttendanceDto> RequestCorrection(
    Guid id,
    [FromBody] AttendanceCorrectionRequest request)
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

        var result =
            _attendanceService.RequestAttendanceCorrection(
                id,
                request,
                loggedInEmployeeId);

        return result.Error switch
        {
            AttendanceOperationError.None =>
                Ok(result.Attendance),

            AttendanceOperationError.NotFound =>
                NotFound("Attendance record not found."),

            AttendanceOperationError.Unauthorized =>
                Forbid(),

            _ =>
                BadRequest("Unable to process correction request.")
        };
    }

    // Deletes an attendance record.
    [Authorize(Roles = "Admin,HrManager")]
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteAttendance(Guid id)
    {
        var result = _attendanceService.DeleteAttendance(id);

        return result.Error switch
        {
            AttendanceOperationError.None => NoContent(),
            AttendanceOperationError.NotFound => NotFound("Attendance record not found."),
            _ => BadRequest("Unable to delete attendance record.")
        };
    }
}