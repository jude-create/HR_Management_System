using HrManagement.Api.Dtos.Holidays;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// HolidaysController handles holiday calendar records.
[ApiController]
[Route("api/[controller]")]
public class HolidaysController : ControllerBase
{
    private readonly IHolidayService _holidayService;

    public HolidaysController(IHolidayService holidayService)
    {
        _holidayService = holidayService;
    }

    // Returns all holidays.
    [HttpGet]
    public ActionResult<IReadOnlyList<HolidayDto>> GetHolidays()
        => Ok(_holidayService.GetHolidays());

    // Creates a new holiday entry.
    [HttpPost]
    public ActionResult<HolidayDto> CreateHoliday([FromBody] HolidayUpsertRequest request)
    {
        var holiday = _holidayService.CreateHoliday(request);
        return holiday is null ? BadRequest("Invalid holiday payload.") : Ok(holiday);
    }

    // Updates an existing holiday.
    [HttpPut("{id:guid}")]
    public ActionResult<HolidayDto> UpdateHoliday(Guid id, [FromBody] HolidayUpsertRequest request)
    {
        var holiday = _holidayService.UpdateHoliday(id, request);
        return holiday is null ? NotFound() : Ok(holiday);
    }

    // Deletes a holiday.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteHoliday(Guid id)
    {
        var deleted = _holidayService.DeleteHoliday(id);
        return deleted ? NoContent() : NotFound("Holiday not found.");
    }
}
