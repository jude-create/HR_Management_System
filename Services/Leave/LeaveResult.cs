using HR_Management_System.Dtos.Leaves;

namespace HR_Management_System.Services.Leave
{
    public enum LeaveOperationError
    {
        None,
        NotFound,
        Unauthorized,
        InvalidDateRange,
        InvalidStatus,
        InvalidDays,
        InvalidLeaveType,
        CrossYearLeave,
        InsufficientLeaveBalance
    }

    public sealed record LeaveResult(
        LeaveOperationError Error,
        LeaveDto? Leave
    )
    {
        public static LeaveResult Success(LeaveDto leave)
            => new(LeaveOperationError.None, leave);

        public static LeaveResult Fail(LeaveOperationError error)
            => new(error, null);
    }

    public sealed record DeleteLeaveResult(
        LeaveOperationError Error
    )
    {
        public static DeleteLeaveResult Ok()
            => new(LeaveOperationError.None);

        public static DeleteLeaveResult Fail(LeaveOperationError error)
            => new(error);
    }

}
