namespace HR_Management_System.Dtos.Common;

public enum CalendarOperationError
{
    None,
    NotFound,
    InvalidType
}

public record CalendarEventResult(
    CalendarOperationError Error,
    Dtos.Calendar.CalendarEventDto? Event
)
{
    public static CalendarEventResult Success(Dtos.Calendar.CalendarEventDto calendarEvent)
        => new(CalendarOperationError.None, calendarEvent);

    public static CalendarEventResult Fail(CalendarOperationError error)
        => new(error, null);
}

public record DeleteCalendarEventResult(
    CalendarOperationError Error
)
{
    public static DeleteCalendarEventResult Ok()
        => new(CalendarOperationError.None);

    public static DeleteCalendarEventResult Fail(CalendarOperationError error)
        => new(error);
}