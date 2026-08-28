namespace HrManagement.Api.Dtos.Attendance;

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

public record AttendanceCorrectionRequest(string Reason);
