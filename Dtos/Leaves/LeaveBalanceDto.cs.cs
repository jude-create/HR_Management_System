namespace HR_Management_System.Dtos.Leave;

public record LeaveBalanceDto(
    Guid EmployeeId,
    int Year,
    int AnnualAllowance,
    int AnnualUsed,
    int AnnualRemaining,
    int SickAllowance,
    int SickUsed,
    int SickRemaining,
    int CasualAllowance,
    int CasualUsed,
    int CasualRemaining
);