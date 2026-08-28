using AutoMapper;
using HrManagement.Api.Dtos.Attendance;
using HrManagement.Api.Dtos.Auth;
using HrManagement.Api.Dtos.Employees;
using HrManagement.Api.Dtos.Holidays;
using HrManagement.Api.Dtos.Notifications;
using HrManagement.Api.Dtos.Payroll;
using HrManagement.Api.Dtos.Recruitment;
using HrManagement.Api.Dtos.Settings;
using HrManagement.Api.Entities;

namespace HrManagement.Api.Mappings;

// AutoMapper profile = the place where we describe how entities become DTOs.
// This keeps the mapping rules in one place instead of repeating them in every service.
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, AuthUserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));

        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.Employees.Count));

        CreateMap<Department, DepartmentDetailDto>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Employees));

        CreateMap<Job, JobDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CandidateCount, opt => opt.MapFrom(src => src.Candidates.Count));

        CreateMap<Candidate, CandidateDto>()
            .ForMember(dest => dest.AppliedForTitle, opt => opt.MapFrom(src => src.Job.Title))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Payroll, PayrollDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
            .ForMember(dest => dest.EmployeeAvatarUrl, opt => opt.MapFrom(src => src.Employee.AvatarUrl))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.Status == NotificationStatus.Read))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.ActionType.ToString()));

        CreateMap<Holiday, HolidayDto>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.Date.DayOfWeek.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<Attendance, AttendanceDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
            .ForMember(dest => dest.EmployeeAvatarUrl, opt => opt.MapFrom(src => src.Employee.AvatarUrl))
            .ForMember(dest => dest.EmployeeTitle, opt => opt.MapFrom(src => src.Employee.Title))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CorrectionStatus, opt => opt.MapFrom(src => src.CorrectionStatus.ToString()));

        CreateMap<UserSettings, SettingsDto>()
            .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance.ToString()))
            .ForMember(dest => dest.TwoFactor, opt => opt.MapFrom(src => src.TwoFactorEnabled))
            .ForMember(dest => dest.MobilePush, opt => opt.MapFrom(src => src.MobilePushEnabled))
            .ForMember(dest => dest.DesktopNotifications, opt => opt.MapFrom(src => src.DesktopNotificationsEnabled))
            .ForMember(dest => dest.EmailNotifications, opt => opt.MapFrom(src => src.EmailNotificationsEnabled));
    }
}
