using HR_Management_System.Dtos.Settings;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// SettingsController exposes the current user's preferences.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    // Gets the current user's settings.
    [HttpGet]
    public ActionResult<SettingsDto> GetSettings()
    {
        try
        {
            return Ok(_settingsService.GetSettings());
        }
        catch (CurrentUserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Updates the current user's settings.
    [HttpPut]
    public ActionResult<SettingsDto> UpdateSettings([FromBody] SettingsUpdateRequest request)
    {
        try
        {
            return Ok(_settingsService.UpdateSettings(request));
        }
        catch (CurrentUserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}