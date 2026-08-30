using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Entities;
using HR_Management_System.Services;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// EmployeeService is responsible for employee CRUD and list pagination.
public interface IEmployeeService
{
    PagedResponse<EmployeeDto> GetEmployees(int page, int pageSize);
    EmployeeDto? GetEmployee(Guid id);
    EmployeeResult CreateEmployee(EmployeeUpsertRequest request);
    EmployeeResult UpdateEmployee(Guid id, EmployeeUpsertRequest request);
    DeleteResult DeleteEmployee(Guid id);
}

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

    public EmployeeResult CreateEmployee(EmployeeUpsertRequest request)
    {
        if (!HrServiceSupport.TryResolveEmployeeType(request.Type, out var type))
        {
            return EmployeeResult.Fail(EmployeeOperationError.InvalidType);
        }

        if (!HrServiceSupport.TryResolveEmployeeStatus(request.Status, out var status))
        {
            return EmployeeResult.Fail(EmployeeOperationError.InvalidStatus);
        }

        var department = _context.Departments.FirstOrDefault(x => x.Id == request.DepartmentId);
        if (department is null)
        {
            return EmployeeResult.Fail(EmployeeOperationError.InvalidDepartment);
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
        return EmployeeResult.Success(_mapper.Map<EmployeeDto>(employee));
    }

    public EmployeeResult UpdateEmployee(Guid id, EmployeeUpsertRequest request)
    {
        var employee = _context.Employees.FirstOrDefault(x => x.Id == id);
        if (employee is null)
        {
            return EmployeeResult.Fail(EmployeeOperationError.NotFound);
        }

        if (!HrServiceSupport.TryResolveEmployeeType(request.Type, out var type))
        {
            return EmployeeResult.Fail(EmployeeOperationError.InvalidType);
        }

        if (!HrServiceSupport.TryResolveEmployeeStatus(request.Status, out var status))
        {
            return EmployeeResult.Fail(EmployeeOperationError.InvalidStatus);
        }

        var newDepartment = _context.Departments.FirstOrDefault(x => x.Id == request.DepartmentId);
        if (newDepartment is null)
        {
            return EmployeeResult.Fail(EmployeeOperationError.InvalidDepartment);
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
        return EmployeeResult.Success(_mapper.Map<EmployeeDto>(employee));
    }

    public DeleteResult DeleteEmployee(Guid id)
    {
        var employee = _context.Employees.FirstOrDefault(x => x.Id == id);
        if (employee is null)
        {
            return DeleteResult.Fail(EmployeeOperationError.NotFound);
        }

        var hasPayroll = _context.Payrolls.Any(x => x.EmployeeId == id);
        var hasAttendance = _context.AttendanceRecords.Any(x => x.EmployeeId == id);
        if (hasPayroll || hasAttendance)
        {
            return DeleteResult.Fail(EmployeeOperationError.HasDependencies);
        }

        _context.Employees.Remove(employee);
        _context.SaveChanges();
        return DeleteResult.Ok();
    }
}