using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Leaves;

public record LeaveDto(
    Guid Id,
    Guid EmployeeId,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    int Days,
    string? ReportingManager,
    string Status
);

public record CreateLeaveRequest(
    [Required]
    string LeaveType,

    [Required]
    DateOnly StartDate,

    [Required]
    DateOnly EndDate,

    string? ReportingManager,

    string? Status
);

public record UpdateLeaveRequest(
    [Required]
    string LeaveType,

    [Required]
    DateOnly StartDate,

    [Required]
    DateOnly EndDate,

    string? ReportingManager,

    [Required]
    string Status
);