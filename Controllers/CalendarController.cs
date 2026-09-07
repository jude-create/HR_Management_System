using HR_Management_System.Dtos.Calendar;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<CalendarEventDto>> GetEvents(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        return Ok(
            _calendarService.GetEvents(fromDate, toDate)
        );
    }

    [HttpPost]
    public ActionResult<CalendarEventDto> CreateEvent(
        [FromBody] CalendarEventCreateRequest request)
    {
        var result = _calendarService.CreateEvent(request);

        return result.Error switch
        {
            CalendarOperationError.None =>
                Ok(result.Event),

            CalendarOperationError.InvalidType =>
                BadRequest("Invalid calendar event type."),

            _ =>
                BadRequest("Invalid calendar event payload.")
        };
    }

    [HttpPut("{id:guid}")]
    public ActionResult<CalendarEventDto> UpdateEvent(
        Guid id,
        [FromBody] CalendarEventUpdateRequest request)
    {
        var result = _calendarService.UpdateEvent(id, request);

        return result.Error switch
        {
            CalendarOperationError.None =>
                Ok(result.Event),

            CalendarOperationError.NotFound =>
                NotFound("Calendar event not found."),

            CalendarOperationError.InvalidType =>
                BadRequest("Invalid calendar event type."),

            _ =>
                BadRequest("Invalid calendar event payload.")
        };
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEvent(Guid id)
    {
        var result = _calendarService.DeleteEvent(id);

        return result.Error switch
        {
            CalendarOperationError.None =>
                NoContent(),

            CalendarOperationError.NotFound =>
                NotFound("Calendar event not found."),

            _ =>
                BadRequest("Unable to delete calendar event.")
        };
    }
}