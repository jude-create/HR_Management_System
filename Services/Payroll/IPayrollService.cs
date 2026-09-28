using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Payroll;

namespace HR_Management_System.Services.Payroll
{
    public interface IPayrollService
    {
        PagedResponse<PayrollDto> GetPayrolls(
            string? period,
            int page,
            int pageSize
        );
        IReadOnlyList<PayrollDto> GeneratePayrolls(PayrollGenerateRequest request);
        ApiMessageResponse ExportPayroll(PayrollExportRequest request);
        DeletePayrollResult DeletePayroll(Guid id);
    }

}
