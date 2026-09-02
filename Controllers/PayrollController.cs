using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Payroll;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// PayrollController exposes payroll list, generate, export, and delete endpoints.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    // Returns payroll records.
    [HttpGet]
    public ActionResult<IReadOnlyList<PayrollDto>> GetPayrolls()
        => Ok(_payrollService.GetPayrolls());

    // Generates payroll records for the requested period.
    [HttpPost("generate")]
    public ActionResult<IReadOnlyList<PayrollDto>> GeneratePayroll([FromBody] PayrollGenerateRequest request)
        => Ok(_payrollService.GeneratePayrolls(request));

    // Returns a placeholder message for payroll export.
    [HttpGet("export")]
    public ActionResult<ApiMessageResponse> ExportPayroll([FromQuery] string period, [FromQuery] string format = "csv")
        => Ok(_payrollService.ExportPayroll(new PayrollExportRequest(period, format)));

    // Deletes a payroll record.
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public IActionResult DeletePayroll(Guid id)
    {
        var result = _payrollService.DeletePayroll(id);

        return result.Error switch
        {
            PayrollOperationError.None => NoContent(),
            PayrollOperationError.NotFound => NotFound("Payroll not found."),
            _ => BadRequest("Unable to delete payroll.")
        };
    }
}