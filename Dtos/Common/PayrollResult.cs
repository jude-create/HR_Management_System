namespace HR_Management_System.Dtos.Common;

public enum PayrollOperationError
{
    None,
    NotFound
}

public sealed record DeletePayrollResult(bool Success, PayrollOperationError Error)
{
    public static DeletePayrollResult Ok() => new(true, PayrollOperationError.None);
    public static DeletePayrollResult Fail(PayrollOperationError error) => new(false, error);
}