using HrManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Register framework services and our app services here.
// This is the startup wiring point for dependency injection.
// ------------------------------------------------------------

builder.Services.AddControllers();

// AutoMapper scans this assembly for Profile classes like MappingProfile.
builder.Services.AddAutoMapper(cfg => { }, typeof(HrManagement.Api.Mappings.MappingProfile).Assembly);

// EF Core  context is scoped per request so each operation gets a clean unit of work.
// Now backed by PostgreSQL instead of the in-memory provider.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in appsettings.json.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Each module gets its own service so the code stays small and focused.
builder.Services.AddScoped<HrManagement.Api.Services.IAuthService, HrManagement.Api.Services.AuthService>();
builder.Services.AddScoped<HrManagement.Api.Services.IEmployeeService, HrManagement.Api.Services.EmployeeService>();
builder.Services.AddScoped<HrManagement.Api.Services.IDepartmentService, HrManagement.Api.Services.DepartmentService>();
builder.Services.AddScoped<HrManagement.Api.Services.IRecruitmentService, HrManagement.Api.Services.RecruitmentService>();
builder.Services.AddScoped<HrManagement.Api.Services.IPayrollService, HrManagement.Api.Services.PayrollService>();
builder.Services.AddScoped<HrManagement.Api.Services.INotificationService, HrManagement.Api.Services.NotificationService>();
builder.Services.AddScoped<HrManagement.Api.Services.IHolidayService, HrManagement.Api.Services.HolidayService>();
builder.Services.AddScoped<HrManagement.Api.Services.IAttendanceService, HrManagement.Api.Services.AttendanceService>();
builder.Services.AddScoped<HrManagement.Api.Services.IDashboardService, HrManagement.Api.Services.DashboardService>();
builder.Services.AddScoped<HrManagement.Api.Services.ISettingsService, HrManagement.Api.Services.SettingsService>();

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