using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Holidays;
using HR_Management_System.Services;
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
        var result = _holidayService.CreateHoliday(request);

        return result.Error switch
        {
            HolidayOperationError.None => Ok(result.Holiday),
            HolidayOperationError.InvalidType => BadRequest("Invalid holiday type."),
            _ => BadRequest("Invalid holiday payload.")
        };
    }

    // Updates an existing holiday.
    [HttpPut("{id:guid}")]
    public ActionResult<HolidayDto> UpdateHoliday(Guid id, [FromBody] HolidayUpsertRequest request)
    {
        var result = _holidayService.UpdateHoliday(id, request);

        return result.Error switch
        {
            HolidayOperationError.None => Ok(result.Holiday),
            HolidayOperationError.NotFound => NotFound("Holiday not found."),
            HolidayOperationError.InvalidType => BadRequest("Invalid holiday type."),
            _ => BadRequest("Invalid holiday payload.")
        };
    }

    // Deletes a holiday.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteHoliday(Guid id)
    {
        var result = _holidayService.DeleteHoliday(id);

        return result.Error switch
        {
            HolidayOperationError.None => NoContent(),
            HolidayOperationError.NotFound => NotFound("Holiday not found."),
            _ => BadRequest("Unable to delete holiday.")
        };
    }
}