using HrManagement.Api.Dtos.Dashboard;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// DashboardController exposes summary metrics for the UI home page.
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // Returns overall HR stats.
    [HttpGet("stats")]
    public ActionResult<DashboardStatsDto> GetStats()
        => Ok(_dashboardService.GetDashboardStats());
}
