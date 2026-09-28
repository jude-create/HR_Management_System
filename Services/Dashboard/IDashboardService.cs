using HR_Management_System.Dtos.Dashboard;

namespace HR_Management_System.Services.Dashboard
{
    public interface IDashboardService
    {
        DashboardStatsDto GetDashboardStats();
    }

}
