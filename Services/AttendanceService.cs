using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Attendance;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

// AttendanceService tracks presence records and correction requests.
public interface IAttendanceService
{
    IReadOnlyList<AttendanceDto> GetAttendance();
    AttendanceDto? RequestAttendanceCorrection(Guid id, AttendanceCorrectionRequest request);
    bool DeleteAttendance(Guid id);
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

    public IReadOnlyList<AttendanceDto> GetAttendance()
        // Most recent attendance records first.
    {
        var attendance = _context.AttendanceRecords
            .Include(x => x.Employee)
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ToList();
        return _mapper.Map<List<AttendanceDto>>(attendance);
    }

    public AttendanceDto? RequestAttendanceCorrection(Guid id, AttendanceCorrectionRequest request)
    {
        // Mark the record as needing review and store the reason.
        var attendance = _context.AttendanceRecords
            .Include(x => x.Employee)
            .FirstOrDefault(x => x.Id == id);
        if (attendance is null)
        {
            return null;
        }

        attendance.CorrectionStatus = CorrectionStatus.Pending;
        attendance.CorrectionReason = request.Reason.Trim();
        _context.SaveChanges();
        return _mapper.Map<AttendanceDto>(attendance);
    }

    public bool DeleteAttendance(Guid id)
    {
        // Attendance rows are removed only if they exist.
        var attendance = _context.AttendanceRecords.FirstOrDefault(x => x.Id == id);
        if (attendance is null)
        {
            return false;
        }

        _context.AttendanceRecords.Remove(attendance);
        return _context.SaveChanges() > 0;
    }

}
