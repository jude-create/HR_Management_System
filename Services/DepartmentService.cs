using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Employees;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// DepartmentService handles department lists, details, create/update, and delete.
public interface IDepartmentService
{
    IReadOnlyList<DepartmentDto> GetDepartments();
    DepartmentDetailDto? GetDepartment(Guid id);
    DepartmentResult CreateDepartment(DepartmentUpsertRequest request);
    DepartmentResult UpdateDepartment(Guid id, DepartmentUpsertRequest request);
    DeleteDepartmentResult DeleteDepartment(Guid id);
}

// This service keeps department logic separate from employee logic.
public sealed class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DepartmentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IReadOnlyList<DepartmentDto> GetDepartments()
    {
        var departments = _context.Departments
            .Include(x => x.Employees)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToList();
        return _mapper.Map<List<DepartmentDto>>(departments);
    }

    public DepartmentDetailDto? GetDepartment(Guid id)
    {
        var department = _context.Departments
            .Include(x => x.Employees)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        return department is null ? null : _mapper.Map<DepartmentDetailDto>(department);
    }

    public DepartmentResult CreateDepartment(DepartmentUpsertRequest request)
    {
        var trimmedName = request.Name.Trim();

        // Prevent two departments from sharing the same name (and therefore slug).
        var nameExists = _context.Departments
            .Any(x => x.Name.ToLower() == trimmedName.ToLower());
        if (nameExists)
        {
            return DepartmentResult.Fail(DepartmentOperationError.DuplicateName);
        }

        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = trimmedName,
            Slug = HrServiceSupport.Slugify(trimmedName)
        };

        _context.Departments.Add(department);
        _context.SaveChanges();
        return DepartmentResult.Success(_mapper.Map<DepartmentDto>(department));
    }

    public DepartmentResult UpdateDepartment(Guid id, DepartmentUpsertRequest request)
    {
        var department = _context.Departments.FirstOrDefault(x => x.Id == id);
        if (department is null)
        {
            return DepartmentResult.Fail(DepartmentOperationError.NotFound);
        }

        var trimmedName = request.Name.Trim();

        // Allow the department to keep its own name, but block collisions with a different department.
        var nameTakenByOther = _context.Departments
            .Any(x => x.Id != id && x.Name.ToLower() == trimmedName.ToLower());
        if (nameTakenByOther)
        {
            return DepartmentResult.Fail(DepartmentOperationError.DuplicateName);
        }

        department.Name = trimmedName;
        department.Slug = HrServiceSupport.Slugify(trimmedName);

        _context.SaveChanges();
        return DepartmentResult.Success(_mapper.Map<DepartmentDto>(department));
    }

    public DeleteDepartmentResult DeleteDepartment(Guid id)
    {
        var department = _context.Departments.FirstOrDefault(x => x.Id == id);
        if (department is null)
        {
            return DeleteDepartmentResult.Fail(DepartmentOperationError.NotFound);
        }

        var hasMembers = _context.Employees.Any(x => x.DepartmentId == id);
        if (hasMembers)
        {
            return DeleteDepartmentResult.Fail(DepartmentOperationError.HasMembers);
        }

        _context.Departments.Remove(department);
        _context.SaveChanges();
        return DeleteDepartmentResult.Ok();
    }
}