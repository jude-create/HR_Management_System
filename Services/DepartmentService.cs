using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Employees;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

// DepartmentService handles department lists, details, create/update, and delete.
public interface IDepartmentService
{
    IReadOnlyList<DepartmentDto> GetDepartments();
    DepartmentDetailDto? GetDepartment(Guid id);
    DepartmentDto? CreateDepartment(DepartmentUpsertRequest request);
    DepartmentDto? UpdateDepartment(Guid id, DepartmentUpsertRequest request);
    bool DeleteDepartment(Guid id);
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
        // Get one department and all employees that belong to it.
        var department = _context.Departments
            .Include(x => x.Employees)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        if (department is null)
        {
            return null;
        }

        return _mapper.Map<DepartmentDetailDto>(department);
    }

    public DepartmentDto? CreateDepartment(DepartmentUpsertRequest request)
    {
        // Slug is generated automatically from the department name.
        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = HrServiceSupport.Slugify(request.Name)
        };

        _context.Departments.Add(department);
        _context.SaveChanges();
        return _mapper.Map<DepartmentDto>(department);
    }

    public DepartmentDto? UpdateDepartment(Guid id, DepartmentUpsertRequest request)
    {
        // Only update if the department exists.
        var department = _context.Departments.FirstOrDefault(x => x.Id == id);
        if (department is null)
        {
            return null;
        }

        department.Name = request.Name.Trim();
        department.Slug = HrServiceSupport.Slugify(request.Name);
        _context.SaveChanges();
        return _mapper.Map<DepartmentDto>(department);
    }

    public bool DeleteDepartment(Guid id)
    {
        // We block delete if the department still has employees attached.
        var department = _context.Departments.FirstOrDefault(x => x.Id == id);
        if (department is null)
        {
            return false;
        }

        var hasMembers = _context.Employees.Any(x => x.DepartmentId == id);
        if (hasMembers)
        {
            return false;
        }

        _context.Departments.Remove(department);
        return _context.SaveChanges() > 0;
    }
}
