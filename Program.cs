using HR_Management_System.Data;
using HR_Management_System.Mappings;
using HR_Management_System.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Register framework services and our app services here.
// This is the startup wiring point for dependency injection.
// ------------------------------------------------------------

builder.Services.AddControllers();

// AutoMapper scans this assembly for Profile classes like MappingProfile.
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

// EF Core  context is scoped per request so each operation gets a clean unit of work.
// Now backed by PostgreSQL instead of the in-memory provider.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in appsettings.json.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Each module gets its own service so the code stays small and focused.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

// Swagger gives us a simple UI for testing the API during development.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------------------------------------------------
// Database setup: create schema (if needed) and seed demo data.
// EnsureCreated() builds tables directly from the EF model —
// fine for local dev; switch to Migrations later if the schema
// needs to evolve with version history.
// ------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
   // context.Database.EnsureCreated();
    AppDbSeeder.Seed(context);
}

// Only expose Swagger in development so production stays clean.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Authorization middleware is here for future auth protection.
app.UseAuthorization();

// Connect controller routes like /api/employees to the controller classes.
app.MapControllers();

app.Run();