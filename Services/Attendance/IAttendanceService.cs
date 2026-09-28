using HR_Management_System.Dtos.Attendance;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Entities.Enums;

namespace HR_Management_System.Services.Attendance
{
    public interface IAttendanceService
    {
        PagedResponse<AttendanceDto> GetAttendance(
            int page,
            int pageSize,
            string? search,
            DateOnly? date,
            DateOnly? fromDate,
            DateOnly? toDate
        );

        PagedResponse<AttendanceDto> GetEmployeeAttendance(
            Guid employeeId,
            int page,
            int pageSize,
            DateOnly? fromDate,
            DateOnly? toDate
        );

        PagedResponse<AttendanceDto> GetMyAttendance(
            Guid employeeId,
            int page,
            int pageSize,
            DateOnly? fromDate,
            DateOnly? toDate
        );

        AttendanceResult CheckIn(
            Guid employeeId,
            AttendanceType type
        );

        AttendanceResult CheckOut(
            Guid employeeId
        );

        AttendanceResult RequestAttendanceCorrection(
            Guid id,
            AttendanceCorrectionRequest request,
            Guid employeeId
        );

        DeleteAttendanceResult DeleteAttendance(Guid id);
    }
}
