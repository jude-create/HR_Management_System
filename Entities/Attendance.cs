namespace HrManagement.Api.Entities;

public class Attendance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public DateOnly Date { get; set; }
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
    public AttendanceType Type { get; set; }
    public AttendanceStatus Status { get; set; }

    public CorrectionStatus CorrectionStatus { get; set; } = CorrectionStatus.None;
    public string? CorrectionReason { get; set; }
}
