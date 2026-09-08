using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

public interface IEmployeeService
{
    PagedResponse<EmployeeDto> GetEmployees(
        int page,
        int pageSize);

    EmployeeDto? GetEmployee(Guid id);

    EmployeeResult CreateEmployee(
        CreateEmployeeRequest request);

    EmployeeResult UpdateEmployee(
        Guid id,
        UpdateEmployeeRequest request);

    DeleteResult DeleteEmployee(Guid id);
}

public sealed class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEmployeeNumberGenerator _employeeNumberGenerator;

    public EmployeeService(
        AppDbContext context,
        IMapper mapper,
        IEmployeeNumberGenerator employeeNumberGenerator)
    {
        _context = context;
        _mapper = mapper;
        _employeeNumberGenerator = employeeNumberGenerator;
    }

    public PagedResponse<EmployeeDto> GetEmployees(
        int page,
        int pageSize)
    {
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Employees
            .Include(x => x.Department)
            .Include(x => x.Links)
            .Include(x => x.Documents)
            .AsNoTracking();

        var totalCount = query.Count();

        var totalPages = Math.Max(
            (int)Math.Ceiling(
                totalCount / (double)safePageSize),
            1);

        var employees = query
            .OrderBy(x => x.Name)
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToList();

        var data = _mapper
            .Map<List<EmployeeDto>>(employees);

        return new PagedResponse<EmployeeDto>(
            data,
            new PageMeta(
                safePage,
                safePageSize,
                totalCount,
                totalPages));
    }

    public EmployeeDto? GetEmployee(Guid id)
    {
        var employee = _context.Employees
            .Include(x => x.Department)
            .Include(x => x.Links)
            .Include(x => x.Documents)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        return employee is null
            ? null
            : _mapper.Map<EmployeeDto>(employee);
    }

    public EmployeeResult CreateEmployee(
        CreateEmployeeRequest request)
    {
        if (!HrServiceSupport.TryResolveEmployeeType(
                request.Type,
                out var type))
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.InvalidType);
        }

        if (!HrServiceSupport.TryResolveEmployeeStatus(
                request.Status,
                out var status))
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.InvalidStatus);
        }

        var department = _context.Departments
            .FirstOrDefault(x => x.Id == request.DepartmentId);

        if (department is null)
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.InvalidDepartment);
        }

        var emailExists = _context.Employees
            .Any(x => x.Email.ToLower() == request.Email.ToLower());

        if (emailExists)
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.DuplicateEmail);
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),

            EmployeeNumber =
                _employeeNumberGenerator.Generate(),

            Name = request.Name.Trim(),

            Email = request.Email.Trim(),

            Mobile = request.Mobile?.Trim(),

            DateOfBirth = request.DateOfBirth,

            Gender = request.Gender?.Trim(),

            Nationality = request.Nationality?.Trim(),

            Address = request.Address?.Trim(),

            City = request.City?.Trim(),

            State = request.State?.Trim(),

            ZipCode = request.ZipCode?.Trim(),

            AvatarUrl = request.AvatarUrl,

            Title = request.Title.Trim(),

            Type = type,

            Status = status,

            JoiningDate =
                request.JoiningDate ?? DateTime.UtcNow,

            OfficeLocation =
                request.OfficeLocation?.Trim(),

            DepartmentId = request.DepartmentId,

            Department = department
        };

        // Create employee links if supplied
        if (request.Links is not null)
        {
            employee.Links = new EmployeeLinks
            {
                Id = Guid.NewGuid(),

                EmployeeId = employee.Id,

                SlackId =
                    request.Links.SlackId?.Trim(),

                SkypeId =
                    request.Links.SkypeId?.Trim(),

                GithubId =
                    request.Links.GithubId?.Trim()
            };
        }

        _context.Employees.Add(employee);

        _context.SaveChanges();

        return EmployeeResult.Success(
            _mapper.Map<EmployeeDto>(employee));
    }

    public EmployeeResult UpdateEmployee(
        Guid id,
        UpdateEmployeeRequest request)
    {
        var employee = _context.Employees
            .Include(x => x.Links)
            .FirstOrDefault(x => x.Id == id);

        if (employee is null)
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.NotFound);
        }

        if (!HrServiceSupport.TryResolveEmployeeType(
                request.Type,
                out var type))
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.InvalidType);
        }

        if (!HrServiceSupport.TryResolveEmployeeStatus(
                request.Status,
                out var status))
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.InvalidStatus);
        }

        var department = _context.Departments
            .FirstOrDefault(x => x.Id == request.DepartmentId);

        if (department is null)
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.InvalidDepartment);
        }

        var emailExists = _context.Employees.Any(x =>
            x.Id != id &&
            x.Email.ToLower() ==
            request.Email.ToLower());

        if (emailExists)
        {
            return EmployeeResult.Fail(
                EmployeeOperationError.DuplicateEmail);
        }

        employee.Name = request.Name.Trim();

        employee.Email = request.Email.Trim();

        employee.Mobile = request.Mobile?.Trim();

        employee.DateOfBirth = request.DateOfBirth;

        employee.Gender = request.Gender?.Trim();

        employee.Nationality = request.Nationality?.Trim();

        employee.Address = request.Address?.Trim();

        employee.City = request.City?.Trim();

        employee.State = request.State?.Trim();

        employee.ZipCode = request.ZipCode?.Trim();

        employee.AvatarUrl = request.AvatarUrl;

        employee.Title = request.Title.Trim();

        employee.Type = type;

        employee.Status = status;

        if (request.JoiningDate.HasValue)
        {
            employee.JoiningDate =
                request.JoiningDate.Value;
        }

        employee.OfficeLocation =
            request.OfficeLocation?.Trim();

        employee.DepartmentId =
            request.DepartmentId;

        employee.Department = department;

        // Update links
        if (request.Links is not null)
        {
            if (employee.Links is null)
            {
                employee.Links = new EmployeeLinks
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id
                };
            }

            employee.Links.SlackId =
                request.Links.SlackId?.Trim();

            employee.Links.SkypeId =
                request.Links.SkypeId?.Trim();

            employee.Links.GithubId =
                request.Links.GithubId?.Trim();
        }

        _context.SaveChanges();

        // Reload complete employee
        var updatedEmployee = _context.Employees
            .Include(x => x.Department)
            .Include(x => x.Links)
            .Include(x => x.Documents)
            .AsNoTracking()
            .First(x => x.Id == id);

        return EmployeeResult.Success(
            _mapper.Map<EmployeeDto>(updatedEmployee));
    }

    public DeleteResult DeleteEmployee(Guid id)
    {
        var employee = _context.Employees
            .FirstOrDefault(x => x.Id == id);

        if (employee is null)
        {
            return DeleteResult.Fail(
                EmployeeOperationError.NotFound);
        }

        var hasPayroll =
            _context.Payrolls
                .Any(x => x.EmployeeId == id);

        var hasAttendance =
            _context.AttendanceRecords
                .Any(x => x.EmployeeId == id);

        if (hasPayroll || hasAttendance)
        {
            return DeleteResult.Fail(
                EmployeeOperationError.HasDependencies);
        }

        _context.Employees.Remove(employee);

        _context.SaveChanges();

        return DeleteResult.Ok();
    }
}