using HR_Management_System.Dtos.Attendance;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Services;
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
        var result = _attendanceService.RequestAttendanceCorrection(id, request);

        return result.Error switch
        {
            AttendanceOperationError.None => Ok(result.Attendance),
            AttendanceOperationError.NotFound => NotFound("Attendance record not found."),
            _ => BadRequest("Unable to process correction request.")
        };
    }

    // Deletes an attendance record.
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