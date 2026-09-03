using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Attendance;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HR_Management_System.Services;

// AttendanceService tracks presence records and correction requests.
public interface IAttendanceService
{
    PagedResponse<AttendanceDto> GetAttendance(
        int page,
        int pageSize,
        string? search,
        DateOnly? date
    );

    AttendanceResult RequestAttendanceCorrection(
        Guid id,
        AttendanceCorrectionRequest request
    );

    DeleteAttendanceResult DeleteAttendance(Guid id);
}

// This module is separate because attendance usually grows with approval logic later.
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
    DateOnly? date)
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

        // Filter by date
        if (date.HasValue)
        {
            query = query.Where(x => x.Date == date.Value);
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

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)pageSize
        );

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

    public AttendanceResult RequestAttendanceCorrection(Guid id, AttendanceCorrectionRequest request)
    {
        var attendance = _context.AttendanceRecords
            .Include(x => x.Employee)
            .FirstOrDefault(x => x.Id == id);
        if (attendance is null)
        {
            return AttendanceResult.Fail(AttendanceOperationError.NotFound);
        }

        attendance.CorrectionStatus = CorrectionStatus.Pending;
        attendance.CorrectionReason = request.Reason.Trim();
        _context.SaveChanges();
        return AttendanceResult.Success(_mapper.Map<AttendanceDto>(attendance));
    }

    public DeleteAttendanceResult DeleteAttendance(Guid id)
    {
        var attendance = _context.AttendanceRecords.FirstOrDefault(x => x.Id == id);
        if (attendance is null)
        {
            return DeleteAttendanceResult.Fail(AttendanceOperationError.NotFound);
        }

        _context.AttendanceRecords.Remove(attendance);
        _context.SaveChanges();
        return DeleteAttendanceResult.Ok();
    }
}