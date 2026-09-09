namespace HR_Management_System.Entities;

public class Leave
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public required string LeaveType { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public int Days { get; set; }

    public string? ReportingManager { get; set; }

    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
}