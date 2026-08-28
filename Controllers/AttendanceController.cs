using HrManagement.Api.Dtos.Attendance;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// AttendanceController manages attendance list and correction requests.
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // Returns the latest attendance entries.
    [HttpGet]
    public ActionResult<IReadOnlyList<AttendanceDto>> GetAttendance()
        => Ok(_attendanceService.GetAttendance());

    // Marks an attendance record for correction review.
    [HttpPost("{id:guid}/correction")]
    public ActionResult<AttendanceDto> RequestCorrection(Guid id, [FromBody] AttendanceCorrectionRequest request)
    {
        var attendance = _attendanceService.RequestAttendanceCorrection(id, request);
        return attendance is null ? NotFound() : Ok(attendance);
    }

    // Deletes an attendance record.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteAttendance(Guid id)
    {
        var deleted = _attendanceService.DeleteAttendance(id);
        return deleted ? NoContent() : NotFound("Attendance record not found.");
    }
}
