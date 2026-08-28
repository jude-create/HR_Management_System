namespace HrManagement.Api.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
    public NotificationActionType ActionType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
