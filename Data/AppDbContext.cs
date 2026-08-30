using System.Text.Json;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HR_Management_System.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Payroll> Payrolls => Set<Payroll>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<Attendance> AttendanceRecords => Set<Attendance>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();

    public Guid CurrentUserId => DemoSeedData.AdminUserId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var stringListComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        modelBuilder.Entity<User>()
            .Property(x => x.Permissions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(stringListComparer);

        modelBuilder.Entity<Job>()
            .Property(x => x.Roles)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(stringListComparer);

        modelBuilder.Entity<Department>()
            .HasMany(x => x.Employees)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasMany(x => x.AttendanceRecords)
            .WithOne(x => x.Employee)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasMany(x => x.PayrollRecords)
            .WithOne(x => x.Employee)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Job>()
            .HasMany(x => x.Candidates)
            .WithOne(x => x.Job)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(x => x.Settings)
            .WithOne(x => x.User)
            .HasForeignKey<UserSettings>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}