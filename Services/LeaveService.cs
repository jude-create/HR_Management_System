using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Leaves;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

public interface ILeaveService
{
    PagedResponse<LeaveDto> GetEmployeeLeaves(
        Guid employeeId,
        int page,
        int pageSize
    );

    LeaveResult CreateLeave(
        Guid employeeId,
        CreateLeaveRequest request
    );

    LeaveResult UpdateLeave(
        Guid id,
        UpdateLeaveRequest request
    );

    DeleteLeaveResult DeleteLeave(Guid id);
}

public enum LeaveOperationError
{
    None,
    NotFound,
    InvalidDateRange,
    InvalidStatus,
    InvalidDays
}

public sealed record LeaveResult(
    LeaveOperationError Error,
    LeaveDto? Leave
)
{
    public static LeaveResult Success(LeaveDto leave)
        => new(LeaveOperationError.None, leave);

    public static LeaveResult Fail(LeaveOperationError error)
        => new(error, null);
}

public sealed record DeleteLeaveResult(
    LeaveOperationError Error
)
{
    public static DeleteLeaveResult Ok()
        => new(LeaveOperationError.None);

    public static DeleteLeaveResult Fail(LeaveOperationError error)
        => new(error);
}

public sealed class LeaveService : ILeaveService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public LeaveService(
        AppDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public PagedResponse<LeaveDto> GetEmployeeLeaves(
        Guid employeeId,
        int page,
        int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Leaves
            .Include(x => x.Employee)
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.StartDate);

        var totalCount = query.Count();

        var leaves = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var data = _mapper.Map<List<LeaveDto>>(leaves);

        var totalPages = totalCount == 0
            ? 1
            : (int)Math.Ceiling(
                totalCount / (double)pageSize
            );

        var meta = new PageMeta(
            page,
            pageSize,
            totalCount,
            totalPages
        );

        return new PagedResponse<LeaveDto>(
            data,
            meta
        );
    }

    public LeaveResult CreateLeave(
        Guid employeeId,
        CreateLeaveRequest request)
    {
        var employeeExists = _context.Employees
            .Any(x => x.Id == employeeId);

        if (!employeeExists)
        {
            return LeaveResult.Fail(
                LeaveOperationError.NotFound
            );
        }

        if (request.EndDate < request.StartDate)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidDateRange
            );
        }

        var days =
            request.EndDate.DayNumber -
            request.StartDate.DayNumber +
            1;

        if (days <= 0)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidDays
            );
        }

        var status = LeaveStatus.Pending;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<LeaveStatus>(
                request.Status,
                true,
                out status))
            {
                return LeaveResult.Fail(
                    LeaveOperationError.InvalidStatus
                );
            }
        }

        var leave = new Leave
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveType = request.LeaveType.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Days = days,
            ReportingManager =
                string.IsNullOrWhiteSpace(request.ReportingManager)
                    ? null
                    : request.ReportingManager.Trim(),
            Status = status
        };

        _context.Leaves.Add(leave);
        _context.SaveChanges();

        var completeLeave = _context.Leaves
            .Include(x => x.Employee)
            .First(x => x.Id == leave.Id);

        return LeaveResult.Success(
            _mapper.Map<LeaveDto>(completeLeave)
        );
    }

    public LeaveResult UpdateLeave(
        Guid id,
        UpdateLeaveRequest request)
    {
        var leave = _context.Leaves
            .FirstOrDefault(x => x.Id == id);

        if (leave is null)
        {
            return LeaveResult.Fail(
                LeaveOperationError.NotFound
            );
        }

        if (request.EndDate < request.StartDate)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidDateRange
            );
        }

        if (!Enum.TryParse<LeaveStatus>(
            request.Status,
            true,
            out var status))
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidStatus
            );
        }

        var days =
            request.EndDate.DayNumber -
            request.StartDate.DayNumber +
            1;

        leave.LeaveType = request.LeaveType.Trim();
        leave.StartDate = request.StartDate;
        leave.EndDate = request.EndDate;
        leave.Days = days;
        leave.ReportingManager =
            string.IsNullOrWhiteSpace(request.ReportingManager)
                ? null
                : request.ReportingManager.Trim();
        leave.Status = status;

        _context.SaveChanges();

        var completeLeave = _context.Leaves
            .Include(x => x.Employee)
            .First(x => x.Id == id);

        return LeaveResult.Success(
            _mapper.Map<LeaveDto>(completeLeave)
        );
    }

    public DeleteLeaveResult DeleteLeave(Guid id)
    {
        var leave = _context.Leaves
            .FirstOrDefault(x => x.Id == id);

        if (leave is null)
        {
            return DeleteLeaveResult.Fail(
                LeaveOperationError.NotFound
            );
        }

        _context.Leaves.Remove(leave);
        _context.SaveChanges();

        return DeleteLeaveResult.Ok();
    }
}