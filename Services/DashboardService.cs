using HR_Management_System.Data;
using HR_Management_System.Dtos.Dashboard;
using HR_Management_System.Entities;

namespace HR_Management_System.Services;

// DashboardService calculates summary numbers for the home page.
public interface IDashboardService
{
    DashboardStatsDto GetDashboardStats();
}

// These values are computed from other modules instead of being stored separately.
public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public DashboardStatsDto GetDashboardStats()
    {
        // Attendance rate is calculated from attendance records in the store.
        var totalAttendance = _context.AttendanceRecords.Count();
        var nonAbsentAttendance = _context.AttendanceRecords.Count(x => x.Status != AttendanceStatus.Absent);
        var attendanceRate = totalAttendance == 0
            ? 0
            : Math.Round(nonAbsentAttendance * 100d / totalAttendance, 1);

      return new DashboardStatsDto(
     _context.Employees.Count(),
     _context.Departments.Count(),
     attendanceRate,
     _context.Payrolls.Count(x => x.Status == PayrollStatus.Draft || x.Status == PayrollStatus.Processing)
 );
    }
}
