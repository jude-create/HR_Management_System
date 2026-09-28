using HR_Management_System.Dtos.Notifications;

namespace HR_Management_System.Services.Notification
{
    public interface INotificationService
    {
        IReadOnlyList<NotificationDto> GetNotifications(Guid userId);

        NotificationDto? UpdateNotificationStatus(
            Guid id,
            Guid userId,
            NotificationStatusRequest request);

        bool DeleteNotification(Guid id, Guid userId);
    }

}
