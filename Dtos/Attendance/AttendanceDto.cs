using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Attendance;

public record AttendanceDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string? EmployeeAvatarUrl,
    string EmployeeTitle,
    string Type,
    DateOnly Date,
    TimeOnly? CheckIn,
    TimeOnly? CheckOut,
    string Status,
    string CorrectionStatus
);

public record AttendanceCorrectionRequest(
    [ Required, StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 500 characters.")]
    string Reason
);