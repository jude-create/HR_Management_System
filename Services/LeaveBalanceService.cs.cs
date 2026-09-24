using HR_Management_System.Data;
using HR_Management_System.Dtos.Leave;
using HR_Management_System.Entities;

namespace HR_Management_System.Services;

public interface ILeaveBalanceService
{
    LeaveBalanceDto? GetBalance(
        Guid employeeId,
        int year
    );

    LeaveBalanceDto? GetOrCreateBalance(
        Guid employeeId,
        int year
    );
    bool DeductLeave(
    Guid employeeId,
    int year,
    string leaveType,
    int days
);

    bool HasSufficientBalance(
    Guid employeeId,
    int year,
    string leaveType,
    int days
);
}

public class LeaveBalanceService : ILeaveBalanceService
{
    private readonly AppDbContext _context;

    public LeaveBalanceService(AppDbContext context)
    {
        _context = context;
    }

    // This method retrieves the leave balance for a specific employee and year. If the balance does not exist, it returns null.
    public LeaveBalanceDto? GetBalance(
        Guid employeeId,
        int year
    )
    {
        var balance = _context.LeaveBalances
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.Year == year
            );

        if (balance is null)
        {
            return null;
        }

        return new LeaveBalanceDto(
            balance.EmployeeId,
            balance.Year,

            balance.AnnualAllowance,
            balance.AnnualUsed,
            balance.AnnualAllowance - balance.AnnualUsed,

            balance.SickAllowance,
            balance.SickUsed,
            balance.SickAllowance - balance.SickUsed,

            balance.CasualAllowance,
            balance.CasualUsed,
            balance.CasualAllowance - balance.CasualUsed
        );
    }
    // This method retrieves the leave balance for a specific employee and year. If the balance does not exist, it creates a new balance with default allowances and returns it.
    public LeaveBalanceDto? GetOrCreateBalance(
    Guid employeeId,
    int year)
    {
        var existingBalance = _context.LeaveBalances
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.Year == year
            );

        if (existingBalance is not null)
        {
            return GetBalance(employeeId, year);
        }

        var employeeExists = _context.Employees
            .Any(x => x.Id == employeeId);

        if (!employeeExists)
        {
            return null;
        }

        var balance = new LeaveBalance
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Year = year,

            AnnualAllowance =
                LeaveBalanceDefaults.AnnualAllowance,

            AnnualUsed = 0,

            SickAllowance =
                LeaveBalanceDefaults.SickAllowance,

            SickUsed = 0,

            CasualAllowance =
                LeaveBalanceDefaults.CasualAllowance,

            CasualUsed = 0
        };

        _context.LeaveBalances.Add(balance);
        _context.SaveChanges();

        return GetBalance(employeeId, year);
    }

    //
    public bool DeductLeave(
    Guid employeeId,
    int year,
    string leaveType,
    int days)
    {
        var balance = _context.LeaveBalances
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.Year == year
            );

        if (balance is null)
        {
            return false;
        }

        switch (leaveType.Trim().ToLower())
        {
            case "annual":
            case "annual leave":
                if (balance.AnnualAllowance - balance.AnnualUsed < days)
                {
                    return false;
                }

                balance.AnnualUsed += days;
                break;

            case "sick":
            case "sick leave":
                if (balance.SickAllowance - balance.SickUsed < days)
                {
                    return false;
                }

                balance.SickUsed += days;
                break;

            case "casual":
            case "casual leave":
                if (balance.CasualAllowance - balance.CasualUsed < days)
                {
                    return false;
                }

                balance.CasualUsed += days;
                break;

            default:
                return false;
        }
        return true;

    }
    public bool HasSufficientBalance(
    Guid employeeId,
    int year,
    string leaveType,
    int days)
    {
        var balance = _context.LeaveBalances
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.Year == year
            );

        if (balance is null)
        {
            return false;
        }

        return leaveType.Trim().ToLower() switch
        {
            "annual" or "annual leave" =>
                balance.AnnualAllowance - balance.AnnualUsed >= days,

            "sick" or "sick leave" =>
                balance.SickAllowance - balance.SickUsed >= days,

            "casual" or "casual leave" =>
                balance.CasualAllowance - balance.CasualUsed >= days,

            _ => false
        };
    }
}