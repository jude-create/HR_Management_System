using HR_Management_System.Data;
using HR_Management_System.Dtos.Dashboard;
using HR_Management_System.Entities;

namespace HR_Management_System.Services;

public interface IDashboardService
{
    DashboardStatsDto GetDashboardStats();
}

public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public DashboardStatsDto GetDashboardStats()
    {
        var totalEmployees = _context.Employees.Count();

        var totalDepartments = _context.Departments.Count();

        var totalAttendance = _context.AttendanceRecords.Count();

        var nonAbsentAttendance = _context.AttendanceRecords.Count(
            x => x.Status != AttendanceStatus.Absent
        );

        var attendanceRate = totalAttendance == 0
            ? 0
            : Math.Round(
                nonAbsentAttendance * 100d / totalAttendance,
                1
            );

        var pendingPayrollCount = _context.Payrolls.Count(
            x =>
                x.Status == PayrollStatus.Draft ||
                x.Status == PayrollStatus.Processing
        );

        return new DashboardStatsDto(
            totalEmployees,
            totalDepartments,
            attendanceRate,
            pendingPayrollCount,
            DateTime.UtcNow
        );
    }
}