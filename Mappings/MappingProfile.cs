using AutoMapper;
using HR_Management_System.Dtos.Attendance;
using HR_Management_System.Dtos.Auth;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Dtos.Holidays;
using HR_Management_System.Dtos.Notifications;
using HR_Management_System.Dtos.Payroll;
using HR_Management_System.Dtos.Recruitment;
using HR_Management_System.Dtos.Settings;
using HR_Management_System.Entities;

namespace HR_Management_System.Mappings;

// AutoMapper profile = the place where we describe how entities become DTOs.
// This keeps the mapping rules in one place instead of repeating them in every service.
// NOTE: All target DTOs are records with positional constructors, so custom-mapped
// fields must use ForCtorParam (not ForMember) — ForMember only works on settable properties.
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, AuthUserDto>()
            .ForCtorParam("Role", opt => opt.MapFrom(src => src.Role.ToString()))
            .ForCtorParam("Permissions", opt => opt.MapFrom(src => src.Permissions));

        CreateMap<Employee, EmployeeDto>()
            .ForCtorParam("Type", opt => opt.MapFrom(src => src.Type.ToString()))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam("DepartmentName", opt => opt.MapFrom(src => src.Department.Name));

        CreateMap<Department, DepartmentDto>()
            .ForCtorParam("MemberCount", opt => opt.MapFrom(src => src.Employees.Count));

        CreateMap<Department, DepartmentDetailDto>()
            .ForCtorParam("Members", opt => opt.MapFrom(src => src.Employees));

        CreateMap<Job, JobDto>()
            .ForCtorParam("Roles", opt => opt.MapFrom(src => src.Roles))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam("CandidateCount", opt => opt.MapFrom(src => src.Candidates.Count));

        CreateMap<Candidate, CandidateDto>()
            .ForCtorParam("AppliedForTitle", opt => opt.MapFrom(src => src.Job.Title))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Payroll, PayrollDto>()
            .ForCtorParam("EmployeeName", opt => opt.MapFrom(src => src.Employee.Name))
            .ForCtorParam("EmployeeAvatarUrl", opt => opt.MapFrom(src => src.Employee.AvatarUrl))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Notification, NotificationDto>()
            .ForCtorParam("IsRead", opt => opt.MapFrom(src => src.Status == NotificationStatus.Read))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam("ActionType", opt => opt.MapFrom(src => src.ActionType.ToString()));

        CreateMap<Holiday, HolidayDto>()
            .ForCtorParam("DayOfWeek", opt => opt.MapFrom(src => src.Date.DayOfWeek.ToString()))
            .ForCtorParam("Type", opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<Attendance, AttendanceDto>()
            .ForCtorParam("EmployeeName", opt => opt.MapFrom(src => src.Employee.Name))
            .ForCtorParam("EmployeeAvatarUrl", opt => opt.MapFrom(src => src.Employee.AvatarUrl))
            .ForCtorParam("EmployeeTitle", opt => opt.MapFrom(src => src.Employee.Title))
            .ForCtorParam("Type", opt => opt.MapFrom(src => src.Type.ToString()))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam("CorrectionStatus", opt => opt.MapFrom(src => src.CorrectionStatus.ToString()));

        CreateMap<UserSettings, SettingsDto>()
            .ForCtorParam("Appearance", opt => opt.MapFrom(src => src.Appearance.ToString()))
            .ForCtorParam("TwoFactor", opt => opt.MapFrom(src => src.TwoFactorEnabled))
            .ForCtorParam("MobilePush", opt => opt.MapFrom(src => src.MobilePushEnabled))
            .ForCtorParam("DesktopNotifications", opt => opt.MapFrom(src => src.DesktopNotificationsEnabled))
            .ForCtorParam("EmailNotifications", opt => opt.MapFrom(src => src.EmailNotificationsEnabled));
    }
}