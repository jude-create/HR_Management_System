namespace HR_Management_System.Entities;

public class CalendarEvent
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? Time { get; set; }

    public CalendarEventType Type { get; set; } = CalendarEventType.Personal;

    public string? Color { get; set; }
}

public enum CalendarEventType
{
    Personal,
    Meeting,
    Reminder,
    Task
}