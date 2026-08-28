namespace HrManagement.Api.Dtos.Dashboard;

// Computed on the fly from Employees/Departments/Attendance/Payroll - not its own table.
public record DashboardStatsDto(
    int TotalEmployees,
    int TotalDepartments,
    double AttendanceRatePercent,
    int PendingPayrollCount
);
