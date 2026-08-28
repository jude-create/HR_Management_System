using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Common;
using HrManagement.Api.Dtos.Employees;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

// EmployeeService is responsible for employee CRUD and list pagination.
public interface IEmployeeService
{
    PagedResponse<EmployeeDto> GetEmployees(int page, int pageSize);
    EmployeeDto? GetEmployee(Guid id);
    EmployeeDto? CreateEmployee(EmployeeUpsertRequest request);
    EmployeeDto? UpdateEmployee(Guid id, EmployeeUpsertRequest request);
    bool DeleteEmployee(Guid id);
}

// It reads and writes employees from the shared in-memory store.
public sealed class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EmployeeService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public PagedResponse<EmployeeDto> GetEmployees(int page, int pageSize)
    {
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        var query = _context.Employees.Include(x => x.Department).AsNoTracking();
        var totalCount = query.Count();
        var totalPages = Math.Max((int)Math.Ceiling(totalCount / (double)safePageSize), 1);

        var data = query
            .OrderBy(x => x.Name)
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .Select(x => _mapper.Map<EmployeeDto>(x))
            .ToList();

        return new PagedResponse<EmployeeDto>(data, new PageMeta(safePage, safePageSize, totalCount, totalPages));
    }

    public EmployeeDto? GetEmployee(Guid id)
    {
        var employee = _context.Employees
            .Include(x => x.Department)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        return employee is null ? null : _mapper.Map<EmployeeDto>(employee);
    }

    public EmployeeDto? CreateEmployee(EmployeeUpsertRequest request)
    {
        // Validate that the enum-like strings and department are valid before saving.
        var department = _context.Departments.FirstOrDefault(x => x.Id == request.DepartmentId);
        if (!HrServiceSupport.TryResolveEmployeeType(request.Type, out var type) ||
            !HrServiceSupport.TryResolveEmployeeStatus(request.Status, out var status) ||
            department is null)
        {
            return null;
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Title = request.Title.Trim(),
            Email = request.Email.Trim(),
            AvatarUrl = request.AvatarUrl,
            Type = type,
            Status = status,
            HiredAt = DateTime.UtcNow,
            DepartmentId = request.DepartmentId,
            Department = department
        };

        _context.Employees.Add(employee);
        _context.SaveChanges();
        return _mapper.Map<EmployeeDto>(employee);
    }

    public EmployeeDto? UpdateEmployee(Guid id, EmployeeUpsertRequest request)
    {
        // Find the employee first; if it does not exist, the update cannot continue.
        var employee = _context.Employees.FirstOrDefault(x => x.Id == id);
        var newDepartment = _context.Departments.FirstOrDefault(x => x.Id == request.DepartmentId);
        if (employee is null ||
            newDepartment is null ||
            !HrServiceSupport.TryResolveEmployeeType(request.Type, out var type) ||
            !HrServiceSupport.TryResolveEmployeeStatus(request.Status, out var status))
        {
            return null;
        }

        employee.Name = request.Name.Trim();
        employee.Title = request.Title.Trim();
        employee.Email = request.Email.Trim();
        employee.AvatarUrl = request.AvatarUrl;
        employee.Type = type;
        employee.Status = status;
        employee.DepartmentId = request.DepartmentId;
        employee.Department = newDepartment;

        _context.SaveChanges();
        return _mapper.Map<EmployeeDto>(employee);
    }

    public bool DeleteEmployee(Guid id)
    {
        // Deletion is blocked if the employee still participates in payroll or attendance.
        var employee = _context.Employees.FirstOrDefault(x => x.Id == id);
        if (employee is null)
        {
            return false;
        }

        var hasPayroll = _context.Payrolls.Any(x => x.EmployeeId == id);
        var hasAttendance = _context.AttendanceRecords.Any(x => x.EmployeeId == id);
        if (hasPayroll || hasAttendance)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        return _context.SaveChanges() > 0;
    }
}
