namespace HrManagement.Api.Dtos.Payroll;

public record PayrollDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string? EmployeeAvatarUrl,
    string Period,
    decimal Ctc,
    decimal Salary,
    decimal Deduction,
    string Status
);

public record PayrollGenerateRequest(string Period);
public record PayrollExportRequest(string Period, string Format = "csv");
