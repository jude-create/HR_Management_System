namespace HR_Management_System.Dtos.Dashboard;

public record DashboardStatsDto(
    int TotalEmployees,
    int TotalDepartments,
    double AttendanceRatePercent,
    int PendingPayrollCount,
    DateTime UpdatedAt
);