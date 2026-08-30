using HR_Management_System.Dtos.Attendance;

namespace HR_Management_System.Dtos.Common;

public enum AttendanceOperationError
{
    None,
    NotFound
}

public sealed record AttendanceResult(AttendanceDto? Attendance, AttendanceOperationError Error)
{
    public static AttendanceResult Success(AttendanceDto dto) => new(dto, AttendanceOperationError.None);
    public static AttendanceResult Fail(AttendanceOperationError error) => new(null, error);
}

public sealed record DeleteAttendanceResult(bool Success, AttendanceOperationError Error)
{
    public static DeleteAttendanceResult Ok() => new(true, AttendanceOperationError.None);
    public static DeleteAttendanceResult Fail(AttendanceOperationError error) => new(false, error);
}