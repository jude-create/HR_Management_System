using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Attendance;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// AttendanceService tracks presence records and correction requests.
public interface IAttendanceService
{
    IReadOnlyList<AttendanceDto> GetAttendance();
    AttendanceResult RequestAttendanceCorrection(Guid id, AttendanceCorrectionRequest request);
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

    public IReadOnlyList<AttendanceDto> GetAttendance()
    {
        // Most recent attendance records first.
        var attendance = _context.AttendanceRecords
            .Include(x => x.Employee)
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ToList();
        return _mapper.Map<List<AttendanceDto>>(attendance);
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