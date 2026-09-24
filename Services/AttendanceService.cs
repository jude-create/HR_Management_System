
using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Attendance;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Entities;
using HR_Management_System.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// AttendanceService tracks presence records and correction requests.
public interface IAttendanceService
{
    PagedResponse<AttendanceDto> GetAttendance(
        int page,
        int pageSize,
        string? search,
        DateOnly? date,
        DateOnly? fromDate,
        DateOnly? toDate
    );

    PagedResponse<AttendanceDto> GetEmployeeAttendance(
        Guid employeeId,
        int page,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate
    );

    PagedResponse<AttendanceDto> GetMyAttendance(
        Guid employeeId,
        int page,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate
    );

    AttendanceResult CheckIn(
        Guid employeeId,
        AttendanceType type
    );

    AttendanceResult CheckOut(
        Guid employeeId
    );

    AttendanceResult RequestAttendanceCorrection(
        Guid id,
        AttendanceCorrectionRequest request,
        Guid? loggedInEmployeeId
    );

    DeleteAttendanceResult DeleteAttendance(Guid id);
}

public sealed class AttendanceService : IAttendanceService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AttendanceService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public PagedResponse<AttendanceDto> GetAttendance(
        int page,
        int pageSize,
        string? search,
        DateOnly? date,
        DateOnly? fromDate,
        DateOnly? toDate
    )
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.AttendanceRecords
            .Include(x => x.Employee)
            .AsNoTracking()
            .AsQueryable();

        // Search by employee name
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();

            query = query.Where(x =>
                x.Employee.Name.ToLower().Contains(searchTerm)
            );
        }

        // Filter by exact date
        if (date.HasValue)
        {
            query = query.Where(x => x.Date == date.Value);
        }

        // Filter by date range
        if (fromDate.HasValue)
        {
            query = query.Where(x => x.Date >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.Date <= toDate.Value);
        }

        // Most recent attendance first
        query = query
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.CheckIn);

        var totalCount = query.Count();

        var attendance = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var data = _mapper.Map<List<AttendanceDto>>(attendance);

        var totalPages = totalCount == 0
            ? 1
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        var meta = new PageMeta(
            page,
            pageSize,
            totalCount,
            totalPages
        );

        return new PagedResponse<AttendanceDto>(
            data,
            meta
        );
    }
    
    // Retrieves the attendance records of a specific employee.
    public PagedResponse<AttendanceDto> GetEmployeeAttendance(
    Guid employeeId,
    int page,
    int pageSize,
    DateOnly? fromDate,
    DateOnly? toDate)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.AttendanceRecords
            .Include(x => x.Employee)
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId);

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.Date >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.Date <= toDate.Value);
        }

        query = query
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.CheckIn);

        var totalCount = query.Count();

        var attendance = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var data = _mapper.Map<List<AttendanceDto>>(attendance);

        var totalPages = totalCount == 0
            ? 1
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        var meta = new PageMeta(
            page,
            pageSize,
            totalCount,
            totalPages
        );

        return new PagedResponse<AttendanceDto>(
            data,
            meta
        );
    }


    // Checks in an employee for the day.
    public AttendanceResult CheckIn(
    Guid employeeId,
    AttendanceType type)
    {
        var employee = _context.Employees
            .FirstOrDefault(x => x.Id == employeeId);

        if (employee is null)
        {
            return AttendanceResult.Fail(
                AttendanceOperationError.NotFound
            );
        }

        var today = DateTimeHelper.Today();
        var currentTime = DateTimeHelper.CurrentTime();

        var attendance = _context.AttendanceRecords
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.Date == today);

        if (attendance is not null && attendance.CheckIn.HasValue)
        {
            return AttendanceResult.Fail(
                AttendanceOperationError.AlreadyCheckedIn
            );
        }

        if (attendance is null)
        {
            attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                Date = today,
                CheckIn = currentTime,
                Type = type,
                Status = AttendanceStatus.Present
            };

            _context.AttendanceRecords.Add(attendance);
        }
        else
        {
            attendance.CheckIn = currentTime;
            attendance.Type = type;
            attendance.Status = AttendanceStatus.Present;
        }

        _context.SaveChanges();

        return AttendanceResult.Success(
            _mapper.Map<AttendanceDto>(attendance)
        );
    }

        // Checks out an employee for the day.
    public AttendanceResult CheckOut(Guid employeeId)
    {
        var today = DateTimeHelper.Today();
        var currentTime = DateTimeHelper.CurrentTime();

        var attendance = _context.AttendanceRecords
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.Date == today);

        if (attendance is null)
        {
            return AttendanceResult.Fail(
                AttendanceOperationError.NotFound
            );
        }

        if (!attendance.CheckIn.HasValue)
        {
            return AttendanceResult.Fail(
                AttendanceOperationError.NotCheckedIn
            );
        }

        if (attendance.CheckOut.HasValue)
        {
            return AttendanceResult.Fail(
                AttendanceOperationError.AlreadyCheckedOut
            );
        }

        attendance.CheckOut = currentTime;

        _context.SaveChanges();

        return AttendanceResult.Success(
            _mapper.Map<AttendanceDto>(attendance)
        );
    }

    // Marks an attendance record for correction review.
    public AttendanceResult RequestAttendanceCorrection(
        Guid id,
        AttendanceCorrectionRequest request,
        Guid? loggedInEmployeeId
        )
    {
        var attendance = _context.AttendanceRecords
            .Include(x => x.Employee)
            .FirstOrDefault(x => x.Id == id);

        if (attendance is null)
        {
            return AttendanceResult.Fail(
                AttendanceOperationError.NotFound
            );
        }
        if (loggedInEmployeeId.HasValue &&
    attendance.EmployeeId != loggedInEmployeeId.Value)
        {
            return AttendanceResult.Fail(
                AttendanceOperationError.Unauthorized
            );
        }

        attendance.CorrectionStatus = CorrectionStatus.Pending;
        attendance.CorrectionReason = request.Reason.Trim();

        _context.SaveChanges();

        return AttendanceResult.Success(
            _mapper.Map<AttendanceDto>(attendance)
        );
    }

    // Retrieves the attendance records of the logged-in employee.
    public PagedResponse<AttendanceDto> GetMyAttendance(
    Guid employeeId,
    int page,
    int pageSize,
    DateOnly? fromDate,
    DateOnly? toDate)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.AttendanceRecords
            .Include(x => x.Employee)
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId);

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.Date >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.Date <= toDate.Value);
        }

        query = query
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.CheckIn);

        var totalCount = query.Count();

        var attendance = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var data = _mapper.Map<List<AttendanceDto>>(attendance);

        var totalPages = totalCount == 0
            ? 1
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        var meta = new PageMeta(
            page,
            pageSize,
            totalCount,
            totalPages
        );

        return new PagedResponse<AttendanceDto>(
            data,
            meta
        );
    }

    public DeleteAttendanceResult DeleteAttendance(Guid id)
    {
        var attendance = _context.AttendanceRecords
            .FirstOrDefault(x => x.Id == id);

        if (attendance is null)
        {
            return DeleteAttendanceResult.Fail(
                AttendanceOperationError.NotFound
            );
        }

        _context.AttendanceRecords.Remove(attendance);
        _context.SaveChanges();

        return DeleteAttendanceResult.Ok();
    }
}
