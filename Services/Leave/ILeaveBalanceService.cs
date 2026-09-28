using HR_Management_System.Dtos.Leave;

namespace HR_Management_System.Services.Leave
{
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

}
