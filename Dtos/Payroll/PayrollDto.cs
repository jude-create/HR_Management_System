using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Payroll;

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

public record PayrollGenerateRequest(
    [ Required, RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])$", ErrorMessage = "Period must be in YYYY-MM format.")]
    string Period
);

public record PayrollExportRequest(
    [ Required, RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])$", ErrorMessage = "Period must be in YYYY-MM format.")]
    string Period,

    [ RegularExpression("^(csv|pdf|xlsx)$", ErrorMessage = "Format must be csv, pdf, or xlsx.")]
    string Format = "csv"
);