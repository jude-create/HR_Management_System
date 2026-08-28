using HrManagement.Api.Entities;

namespace HrManagement.Api.Services;

// This class is our fake database for now.
// It stores seeded data in memory so the API can be tested without a real DB.
public sealed class HrDataStore
{
    // Each list represents one table-like collection.
    public List<User> Users { get; } = new();
    public List<Department> Departments { get; } = new();
    public List<Employee> Employees { get; } = new();
    public List<Job> Jobs { get; } = new();
    public List<Candidate> Candidates { get; } = new();
    public List<Payroll> Payrolls { get; } = new();
    public List<Notification> Notifications { get; } = new();
    public List<Holiday> Holidays { get; } = new();
    public List<Attendance> AttendanceRecords { get; } = new();

    public Guid CurrentUserId { get; } = SeedIds.AdminUserId;

    public HrDataStore()
    {
        // Seed data once when the app starts.
        Seed();
    }

    private void Seed()
    {
        // Departments are the top-level grouping for employees and jobs.
        var engineering = new Department
        {
            Id = SeedIds.EngineeringDepartmentId,
            Name = "Engineering",
            Slug = "engineering"
        };

        var peopleOps = new Department
        {
            Id = SeedIds.PeopleOpsDepartmentId,
            Name = "People Operations",
            Slug = "people-operations"
        };

        var finance = new Department
        {
            Id = SeedIds.FinanceDepartmentId,
            Name = "Finance",
            Slug = "finance"
        };

        // Add the initial departments to the in-memory store.
        Departments.AddRange([engineering, peopleOps, finance]);

        // Users represent login-capable accounts.
        var adminUser = new User
        {
            Id = SeedIds.AdminUserId,
            Name = "Amina Bello",
            Email = "admin@hr.local",
            PasswordHash = HrServiceSupport.HashPassword("Password123!"),
            Role = UserRole.Admin,
            Permissions = ["employees.read", "employees.write", "payroll.manage", "settings.manage"],
            Settings = new UserSettings
            {
                Id = SeedIds.AdminSettingsId,
                UserId = SeedIds.AdminUserId,
                Appearance = AppearanceMode.System,
                Language = "en",
                TwoFactorEnabled = true,
                MobilePushEnabled = true,
                DesktopNotificationsEnabled = true,
                EmailNotificationsEnabled = true
            }
        };

        // A second user gives us more realistic auth data.
        var hrUser = new User
        {
            Id = SeedIds.HrUserId,
            Name = "Tunde Adeyemi",
            Email = "hr@hr.local",
            PasswordHash = HrServiceSupport.HashPassword("Password123!"),
            Role = UserRole.HrManager,
            Permissions = ["employees.read", "employees.write", "recruitment.manage"],
            Settings = new UserSettings
            {
                Id = SeedIds.HrSettingsId,
                UserId = SeedIds.HrUserId,
                Appearance = AppearanceMode.Dark,
                Language = "en",
                TwoFactorEnabled = false,
                MobilePushEnabled = true,
                DesktopNotificationsEnabled = true,
                EmailNotificationsEnabled = false
            }
        };

        Users.AddRange([adminUser, hrUser]);

        // Employees are linked to departments by DepartmentId.
        Employees.AddRange(
            [
                new Employee
                {
                    Id = SeedIds.Employee1Id,
                    Name = "Seyi Johnson",
                    Title = "Senior Backend Engineer",
                    Email = "seyi.johnson@hr.local",
                    AvatarUrl = "https://i.pravatar.cc/160?img=12",
                    Type = EmployeeType.FullTime,
                    Status = EmployeeStatus.Active,
                    HiredAt = DateTime.UtcNow.AddYears(-2),
                    DepartmentId = engineering.Id
                },
                new Employee
                {
                    Id = SeedIds.Employee2Id,
                    Name = "Mariam Yusuf",
                    Title = "People Partner",
                    Email = "mariam.yusuf@hr.local",
                    AvatarUrl = "https://i.pravatar.cc/160?img=32",
                    Type = EmployeeType.FullTime,
                    Status = EmployeeStatus.Active,
                    HiredAt = DateTime.UtcNow.AddYears(-1),
                    DepartmentId = peopleOps.Id
                },
                new Employee
                {
                    Id = SeedIds.Employee3Id,
                    Name = "Kelechi Okafor",
                    Title = "Finance Analyst",
                    Email = "kelechi.okafor@hr.local",
                    AvatarUrl = "https://i.pravatar.cc/160?img=52",
                    Type = EmployeeType.Contract,
                    Status = EmployeeStatus.OnLeave,
                    HiredAt = DateTime.UtcNow.AddMonths(-8),
                    DepartmentId = finance.Id
                }
            ]
        );

        // Jobs belong to departments and can attract candidates.
        Jobs.AddRange(
            [
                new Job
                {
                    Id = SeedIds.Job1Id,
                    Title = "Frontend Engineer",
                    Description = "Build responsive product experiences for the HR portal.",
                    Roles = ["React", "TypeScript", "UI Engineering"],
                    Location = "Remote",
                    SalaryMin = 4000000,
                    SalaryMax = 6500000,
                    Status = JobStatus.Open,
                    DepartmentId = engineering.Id
                },
                new Job
                {
                    Id = SeedIds.Job2Id,
                    Title = "Recruitment Specialist",
                    Description = "Run sourcing, scheduling, and candidate communications.",
                    Roles = ["Talent Acquisition", "Interview Coordination"],
                    Location = "Lagos",
                    SalaryMin = 2500000,
                    SalaryMax = 3800000,
                    Status = JobStatus.OnHold,
                    DepartmentId = peopleOps.Id
                }
            ]
        );

        // Candidates are tied to jobs through JobId.
        Candidates.AddRange(
            [
                new Candidate
                {
                    Id = SeedIds.Candidate1Id,
                    Name = "Grace Ade",
                    AvatarUrl = "https://i.pravatar.cc/160?img=47",
                    Email = "grace.ade@example.com",
                    PhoneNumber = "+2348012345678",
                    AppliedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-4)),
                    Status = CandidateStatus.Screening,
                    JobId = SeedIds.Job1Id
                },
                new Candidate
                {
                    Id = SeedIds.Candidate2Id,
                    Name = "Samuel Dada",
                    AvatarUrl = "https://i.pravatar.cc/160?img=18",
                    Email = "samuel.dada@example.com",
                    PhoneNumber = "+2348098765432",
                    AppliedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-9)),
                    Status = CandidateStatus.Interview,
                    JobId = SeedIds.Job1Id
                },
                new Candidate
                {
                    Id = SeedIds.Candidate3Id,
                    Name = "Ifeoma Nwosu",
                    AvatarUrl = "https://i.pravatar.cc/160?img=24",
                    Email = "ifeoma.nwosu@example.com",
                    PhoneNumber = "+2348076543210",
                    AppliedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                    Status = CandidateStatus.Applied,
                    JobId = SeedIds.Job2Id
                }
            ]
        );

        // Payroll records are attached to employees and period.
        Payrolls.AddRange(
            [
                new Payroll
                {
                    Id = SeedIds.Payroll1Id,
                    EmployeeId = SeedIds.Employee1Id,
                    Period = "2026-08",
                    Ctc = 800000m,
                    Salary = 600000m,
                    Deduction = 200000m,
                    Status = PayrollStatus.Paid,
                    PaidAt = DateTime.UtcNow.AddDays(-1)
                },
                new Payroll
                {
                    Id = SeedIds.Payroll2Id,
                    EmployeeId = SeedIds.Employee2Id,
                    Period = "2026-08",
                    Ctc = 650000m,
                    Salary = 500000m,
                    Deduction = 150000m,
                    Status = PayrollStatus.Processing
                }
            ]
        );

        // Notifications help surface events in the UI.
        Notifications.AddRange(
            [
                new Notification
                {
                    Id = SeedIds.Notification1Id,
                    UserId = adminUser.Id,
                    Title = "Payroll approved",
                    Description = "August payroll was approved and scheduled.",
                    Status = NotificationStatus.Unread,
                    ActionType = NotificationActionType.Approval,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new Notification
                {
                    Id = SeedIds.Notification2Id,
                    UserId = adminUser.Id,
                    Title = "Candidate moved forward",
                    Description = "Grace Ade has been moved to screening.",
                    Status = NotificationStatus.Read,
                    ActionType = NotificationActionType.Info,
                    CreatedAt = DateTime.UtcNow.AddHours(-8)
                },
                new Notification
                {
                    Id = SeedIds.Notification3Id,
                    UserId = adminUser.Id,
                    Title = "Attendance correction pending",
                    Description = "A correction request is waiting for approval.",
                    Status = NotificationStatus.Unread,
                    ActionType = NotificationActionType.Alert,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            ]
        );

        // Holidays are simple calendar entries.
        Holidays.AddRange(
            [
                new Holiday
                {
                    Id = SeedIds.Holiday1Id,
                    Name = "Workers' Day",
                    Date = new DateOnly(2026, 5, 1),
                    Type = HolidayType.Public
                },
                new Holiday
                {
                    Id = SeedIds.Holiday2Id,
                    Name = "Company Retreat",
                    Date = new DateOnly(2026, 9, 18),
                    Type = HolidayType.Company
                },
                new Holiday
                {
                    Id = SeedIds.Holiday3Id,
                    Name = "Founders' Day",
                    Date = new DateOnly(2026, 11, 6),
                    Type = HolidayType.Optional
                }
            ]
        );

        // Attendance records track presence and time stamps.
        AttendanceRecords.AddRange(
            [
                new Attendance
                {
                    Id = SeedIds.Attendance1Id,
                    EmployeeId = SeedIds.Employee1Id,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                    CheckIn = new TimeOnly(8, 3),
                    CheckOut = new TimeOnly(17, 41),
                    Type = AttendanceType.Office,
                    Status = AttendanceStatus.Present,
                    CorrectionStatus = CorrectionStatus.None
                },
                new Attendance
                {
                    Id = SeedIds.Attendance2Id,
                    EmployeeId = SeedIds.Employee2Id,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                    CheckIn = new TimeOnly(8, 50),
                    CheckOut = new TimeOnly(17, 10),
                    Type = AttendanceType.Remote,
                    Status = AttendanceStatus.Late,
                    CorrectionStatus = CorrectionStatus.Pending
                },
                new Attendance
                {
                    Id = SeedIds.Attendance3Id,
                    EmployeeId = SeedIds.Employee3Id,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                    Type = AttendanceType.FieldWork,
                    Status = AttendanceStatus.OnLeave,
                    CorrectionStatus = CorrectionStatus.None
                }
            ]
        );

        // Wire up navigation properties so AutoMapper can read related data later.
        foreach (var department in Departments)
        {
            department.Employees = Employees.Where(x => x.DepartmentId == department.Id).ToList();
        }

        foreach (var employee in Employees)
        {
            employee.Department = Departments.First(x => x.Id == employee.DepartmentId);
        }

        foreach (var job in Jobs)
        {
            job.Department = Departments.First(x => x.Id == job.DepartmentId);
            job.Candidates = Candidates.Where(x => x.JobId == job.Id).ToList();
        }

        foreach (var candidate in Candidates)
        {
            candidate.Job = Jobs.First(x => x.Id == candidate.JobId);
        }

        foreach (var payroll in Payrolls)
        {
            payroll.Employee = Employees.First(x => x.Id == payroll.EmployeeId);
        }

        foreach (var attendance in AttendanceRecords)
        {
            attendance.Employee = Employees.First(x => x.Id == attendance.EmployeeId);
        }

        foreach (var user in Users)
        {
            user.Settings.User = user;
        }
    }

    private static class SeedIds
    {
        public static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid HrUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static readonly Guid AdminSettingsId = Guid.Parse("31111111-1111-1111-1111-111111111111");
        public static readonly Guid HrSettingsId = Guid.Parse("32222222-2222-2222-2222-222222222222");

        public static readonly Guid EngineeringDepartmentId = Guid.Parse("41111111-1111-1111-1111-111111111111");
        public static readonly Guid PeopleOpsDepartmentId = Guid.Parse("42222222-2222-2222-2222-222222222222");
        public static readonly Guid FinanceDepartmentId = Guid.Parse("43333333-3333-3333-3333-333333333333");

        public static readonly Guid Employee1Id = Guid.Parse("51111111-1111-1111-1111-111111111111");
        public static readonly Guid Employee2Id = Guid.Parse("52222222-2222-2222-2222-222222222222");
        public static readonly Guid Employee3Id = Guid.Parse("53333333-3333-3333-3333-333333333333");

        public static readonly Guid Job1Id = Guid.Parse("61111111-1111-1111-1111-111111111111");
        public static readonly Guid Job2Id = Guid.Parse("62222222-2222-2222-2222-222222222222");

        public static readonly Guid Candidate1Id = Guid.Parse("71111111-1111-1111-1111-111111111111");
        public static readonly Guid Candidate2Id = Guid.Parse("72222222-2222-2222-2222-222222222222");
        public static readonly Guid Candidate3Id = Guid.Parse("73333333-3333-3333-3333-333333333333");

        public static readonly Guid Payroll1Id = Guid.Parse("81111111-1111-1111-1111-111111111111");
        public static readonly Guid Payroll2Id = Guid.Parse("82222222-2222-2222-2222-222222222222");

        public static readonly Guid Notification1Id = Guid.Parse("91111111-1111-1111-1111-111111111111");
        public static readonly Guid Notification2Id = Guid.Parse("92222222-2222-2222-2222-222222222222");
        public static readonly Guid Notification3Id = Guid.Parse("93333333-3333-3333-3333-333333333333");

        public static readonly Guid Holiday1Id = Guid.Parse("a1111111-1111-1111-1111-111111111111");
        public static readonly Guid Holiday2Id = Guid.Parse("a2222222-2222-2222-2222-222222222222");
        public static readonly Guid Holiday3Id = Guid.Parse("a3333333-3333-3333-3333-333333333333");

        public static readonly Guid Attendance1Id = Guid.Parse("b1111111-1111-1111-1111-111111111111");
        public static readonly Guid Attendance2Id = Guid.Parse("b2222222-2222-2222-2222-222222222222");
        public static readonly Guid Attendance3Id = Guid.Parse("b3333333-3333-3333-3333-333333333333");
    }
}
