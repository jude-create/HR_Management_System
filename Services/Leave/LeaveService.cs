using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Leaves;
using HR_Management_System.Entities;
using HR_Management_System.Entities.Enums;
using Microsoft.EntityFrameworkCore;

using LeaveEntity = HR_Management_System.Entities.Leave;

namespace HR_Management_System.Services.Leave;



public sealed class LeaveService : ILeaveService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILeaveBalanceService _leaveBalanceService;

    public LeaveService(
        AppDbContext context,
        IMapper mapper,
        ILeaveBalanceService leaveBalanceService)
    {
        _context = context;
        _mapper = mapper;
        _leaveBalanceService = leaveBalanceService;
    }

    // get all leaves
    public PagedResponse<LeaveDto> GetAllLeaves(
        int page,
        int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Leaves
            .Include(x => x.Employee)
            .AsNoTracking()
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

    //get employee leaves
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

    //get my leaves
    public PagedResponse<LeaveDto> GetMyLeaves(
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

    //create leave
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

        var leaveType = request.LeaveType.Trim();

        var normalizedLeaveType = leaveType.EndsWith(
            " Leave",
            StringComparison.OrdinalIgnoreCase)
            ? leaveType[..^6].Trim()
            : leaveType;

        if (!Enum.TryParse<LeaveType>(
                normalizedLeaveType,
                true,
                out _))
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidLeaveType
            );
        }

        if (request.EndDate < request.StartDate)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidDateRange
            );
        }

        if (request.StartDate.Year != request.EndDate.Year)
        {
            return LeaveResult.Fail(
                LeaveOperationError.CrossYearLeave
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

        var hasSufficientBalance =
            _leaveBalanceService.HasSufficientBalance(
                employeeId,
                request.StartDate.Year,
                request.LeaveType,
                days
            );

        if (!hasSufficientBalance)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InsufficientLeaveBalance
            );
        }

        var leave = new LeaveEntity
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveType = leaveType,
            Reason = request.Reason?.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Days = days,
            ReportingManager =
                string.IsNullOrWhiteSpace(request.ReportingManager)
                    ? null
                    : request.ReportingManager.Trim(),
            Status = LeaveStatus.Pending
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
    //update leave
    public LeaveResult UpdateLeave(
        Guid id,
        UpdateLeaveRequest request,
        Guid employeeId
        )
    {
        var leave = _context.Leaves
            .FirstOrDefault(x => x.Id == id);

        if (leave is null)
        {
            return LeaveResult.Fail(
                LeaveOperationError.NotFound
            );
        }
        if (leave.Status != LeaveStatus.Pending)
        {
            return LeaveResult.Fail(
                LeaveOperationError.Unauthorized
            );
        }

        var leaveType = request.LeaveType.Trim();

        var normalizedLeaveType = leaveType.EndsWith(" Leave", StringComparison.OrdinalIgnoreCase)
            ? leaveType[..^6].Trim()
            : leaveType;

        if (!Enum.TryParse<LeaveType>(
                normalizedLeaveType,
                true,
                out _))
        {
            return LeaveResult.Fail(LeaveOperationError.InvalidLeaveType);
        }


        if (leave.EmployeeId != employeeId)
        {
            return LeaveResult.Fail(
                LeaveOperationError.Unauthorized
            );
        }

        if (request.EndDate < request.StartDate)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidDateRange
            );
        }

        if (request.StartDate.Year != request.EndDate.Year)
        {
            return LeaveResult.Fail(
                LeaveOperationError.CrossYearLeave
            );
        }


        var days =
            request.EndDate.DayNumber -
            request.StartDate.DayNumber +
            1;

        leave.LeaveType = request.LeaveType.Trim();
        leave.Reason = request.Reason?.Trim();
        leave.StartDate = request.StartDate;
        leave.EndDate = request.EndDate;
        leave.Days = days;
        leave.ReportingManager =
            string.IsNullOrWhiteSpace(request.ReportingManager)
                ? null
                : request.ReportingManager.Trim();


        _context.SaveChanges();

        var completeLeave = _context.Leaves
            .Include(x => x.Employee)
            .First(x => x.Id == id);

        return LeaveResult.Success(
            _mapper.Map<LeaveDto>(completeLeave)
        );
    }

    //delete leave
    public DeleteLeaveResult DeleteLeave(
    Guid id,
    Guid employeeId)
    {
        var leave = _context.Leaves
            .FirstOrDefault(x => x.Id == id);

        if (leave is null)
        {
            return DeleteLeaveResult.Fail(
                LeaveOperationError.NotFound
            );
        }
        if (leave.Status != LeaveStatus.Pending)
        {
            return DeleteLeaveResult.Fail(
                LeaveOperationError.Unauthorized
            );
        }
        if (leave.EmployeeId != employeeId)
        {
            return DeleteLeaveResult.Fail(
                LeaveOperationError.Unauthorized
            );
        }

        _context.Leaves.Remove(leave);
        _context.SaveChanges();

        return DeleteLeaveResult.Ok();
    }

    //update leave status
    public LeaveResult UpdateLeaveStatus(
    Guid id,
    UpdateLeaveStatusRequest request)
    {
        using var transaction = _context.Database.BeginTransaction();
       
        var leave = _context.Leaves
            .FirstOrDefault(x => x.Id == id);

        if (leave is null)
        {
            return LeaveResult.Fail(
                LeaveOperationError.NotFound
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

        if (status != LeaveStatus.Approved &&
            status != LeaveStatus.Rejected)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidStatus
            );
        }

        if (leave.Status != LeaveStatus.Pending)
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidStatus
            );
        }

        if (status == LeaveStatus.Rejected &&
            string.IsNullOrWhiteSpace(request.RejectionReason))
        {
            return LeaveResult.Fail(
                LeaveOperationError.InvalidStatus
            );
        }
        if (status == LeaveStatus.Approved)
        {
            var balanceDeducted = _leaveBalanceService.DeductLeave(
                leave.EmployeeId,
                leave.StartDate.Year,
                leave.LeaveType,
                leave.Days
            );

            if (!balanceDeducted)
            {
                return LeaveResult.Fail(
                    LeaveOperationError.InsufficientLeaveBalance
                );
            }
        }

      try
{
    leave.Status = status;

    leave.RejectionReason =
        status == LeaveStatus.Rejected
            ? request.RejectionReason!.Trim()
            : null;

    _context.SaveChanges();

    transaction.Commit();
}
catch
{
    transaction.Rollback();

    throw;
}

        var completeLeave = _context.Leaves
            .Include(x => x.Employee)
            .First(x => x.Id == id);

        return LeaveResult.Success(
            _mapper.Map<LeaveDto>(completeLeave)
        );
    }
}