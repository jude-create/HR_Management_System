using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Leaves;

public record LeaveDto(
   Guid Id,
    Guid EmployeeId,
    string LeaveType,
    string? Reason,
    DateOnly StartDate,
    DateOnly EndDate,
    int Days,
    string? ReportingManager,
    string Status,
    string? RejectionReason
);

public record CreateLeaveRequest(
    [Required]
    string LeaveType,

      string? Reason,

    [Required]
    DateOnly StartDate,

    [Required]
    DateOnly EndDate,

    string? ReportingManager

   
);

public record UpdateLeaveRequest(
    [Required]
    string LeaveType,

     string? Reason,

    [Required]
    DateOnly StartDate,

    [Required]
    DateOnly EndDate,

    string? ReportingManager

   
);

public record UpdateLeaveStatusRequest(
    [Required]
    string Status,

    string? RejectionReason
);