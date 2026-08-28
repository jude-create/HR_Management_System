namespace HrManagement.Api.Entities;

// Employee is the internal data model for a staff member.
// This is the object the service layer works with before mapping to an API DTO.
public class Employee
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Title { get; set; } = string.Empty;
    public required string Email { get; set; } 
    public string? AvatarUrl { get; set; }
    // These values are stored as enums internally for safety and consistency.
    public EmployeeType Type { get; set; }
    public EmployeeStatus Status { get; set; }
    public DateTime HiredAt { get; set; }

    // DepartmentId is the link back to the department this employee belongs to.
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Related records that belong to this employee.
    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    public ICollection<Payroll> PayrollRecords { get; set; } = new List<Payroll>();
}
