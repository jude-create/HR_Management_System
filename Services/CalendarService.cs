using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Calendar;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

public interface ICalendarService
{
    IReadOnlyList<CalendarEventDto> GetEvents(
        DateOnly? fromDate,
        DateOnly? toDate
    );

    CalendarEventResult CreateEvent(
        CalendarEventCreateRequest request
    );

    CalendarEventResult UpdateEvent(
        Guid id,
        CalendarEventUpdateRequest request
    );

    DeleteCalendarEventResult DeleteEvent(Guid id);
}

public sealed class CalendarService : ICalendarService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CalendarService(
        AppDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IReadOnlyList<CalendarEventDto> GetEvents(
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        var query = _context.CalendarEvents
            .AsNoTracking()
            .Where(x => x.UserId == _context.CurrentUserId);

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.Date >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.Date <= toDate.Value);
        }

        var events = query
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Time)
            .ToList();

        return _mapper.Map<List<CalendarEventDto>>(events);
    }

    public CalendarEventResult CreateEvent(
        CalendarEventCreateRequest request)
    {
        if (!Enum.TryParse<CalendarEventType>(
                request.Type,
                true,
                out var type))
        {
            return CalendarEventResult.Fail(
                CalendarOperationError.InvalidType);
        }

        var calendarEvent = new CalendarEvent
        {
            Id = Guid.NewGuid(),
            UserId = _context.CurrentUserId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Date = request.Date,
            Time = request.Time,
            Type = type,
            Color = string.IsNullOrWhiteSpace(request.Color)
                ? "#7152F3"
                : request.Color
        };

        _context.CalendarEvents.Add(calendarEvent);
        _context.SaveChanges();

        return CalendarEventResult.Success(
            _mapper.Map<CalendarEventDto>(calendarEvent));
    }

    public CalendarEventResult UpdateEvent(
        Guid id,
        CalendarEventUpdateRequest request)
    {
        var calendarEvent = _context.CalendarEvents
            .FirstOrDefault(x =>
                x.Id == id &&
                x.UserId == _context.CurrentUserId);

        if (calendarEvent is null)
        {
            return CalendarEventResult.Fail(
                CalendarOperationError.NotFound);
        }

        if (!Enum.TryParse<CalendarEventType>(
                request.Type,
                true,
                out var type))
        {
            return CalendarEventResult.Fail(
                CalendarOperationError.InvalidType);
        }

        calendarEvent.Title = request.Title.Trim();
        calendarEvent.Description = request.Description?.Trim();
        calendarEvent.Date = request.Date;
        calendarEvent.Time = request.Time;
        calendarEvent.Type = type;
        calendarEvent.Color = string.IsNullOrWhiteSpace(request.Color)
            ? "#7152F3"
            : request.Color;

        _context.SaveChanges();

        return CalendarEventResult.Success(
            _mapper.Map<CalendarEventDto>(calendarEvent));
    }

    public DeleteCalendarEventResult DeleteEvent(Guid id)
    {
        var calendarEvent = _context.CalendarEvents
            .FirstOrDefault(x =>
                x.Id == id &&
                x.UserId == _context.CurrentUserId);

        if (calendarEvent is null)
        {
            return DeleteCalendarEventResult.Fail(
                CalendarOperationError.NotFound);
        }

        _context.CalendarEvents.Remove(calendarEvent);
        _context.SaveChanges();

        return DeleteCalendarEventResult.Ok();
    }
}