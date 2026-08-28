using HrManagement.Api.Dtos.Settings;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// SettingsController exposes the current user's preferences.
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
        => Ok(_settingsService.GetSettings());

    // Updates the current user's settings.
    [HttpPut]
    public ActionResult<SettingsDto> UpdateSettings([FromBody] SettingsUpdateRequest request)
        => Ok(_settingsService.UpdateSettings(request));
}
