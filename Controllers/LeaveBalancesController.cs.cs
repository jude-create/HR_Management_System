using HR_Management_System.Dtos.Leave;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeaveBalancesController : ControllerBase
{
    private readonly ILeaveBalanceService _leaveBalanceService;

    public LeaveBalancesController(
        ILeaveBalanceService leaveBalanceService)
    {
        _leaveBalanceService = leaveBalanceService;
    }

    [HttpGet("my")]
    public ActionResult<LeaveBalanceDto> GetMyBalance(
        [FromQuery] int year)
    {
        var employeeIdClaim =
            User.FindFirst("employeeId")?.Value;

        if (!Guid.TryParse(
            employeeIdClaim,
            out var employeeId))
        {
            return Forbid();
        }

        var balance = _leaveBalanceService.GetOrCreateBalance(
      employeeId,
      year
  );

        if (balance is null)
        {
            return NotFound(
                "Leave balance not found for this year."
            );
        }

        return Ok(balance);
    }
}