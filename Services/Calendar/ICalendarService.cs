using HR_Management_System.Dtos.Calendar;

namespace HR_Management_System.Services.Calendar
{
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

}
