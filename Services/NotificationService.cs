using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Notifications;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// NotificationService handles the app's alert and notification feed.
public interface INotificationService
{
    IReadOnlyList<NotificationDto> GetNotifications(Guid userId);

    NotificationDto? UpdateNotificationStatus(
        Guid id,
        Guid userId,
        NotificationStatusRequest request);

    bool DeleteNotification(Guid id, Guid userId);
}

// This keeps the notification logic isolated from payroll, auth, and other modules.
public sealed class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public NotificationService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IReadOnlyList<NotificationDto> GetNotifications(Guid userId)
    {
        var notifications = _context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return _mapper.Map<List<NotificationDto>>(notifications);
    }

    public NotificationDto? UpdateNotificationStatus(
    Guid id,
    Guid userId,
    NotificationStatusRequest request)
    {
        // Find the notification only if it belongs to the logged-in user.
        var notification = _context.Notifications
            .FirstOrDefault(x =>
                x.Id == id &&
                x.UserId == userId);

        if (notification is null ||
            !Enum.TryParse<NotificationStatus>(
                request.Status,
                true,
                out var status))
        {
            return null;
        }

        notification.Status = status;

        _context.SaveChanges();

        return _mapper.Map<NotificationDto>(notification);
    }

    public bool DeleteNotification(Guid id, Guid userId)
    {
        // Only find the notification if it belongs to the logged-in user.
        var notification = _context.Notifications
            .FirstOrDefault(x =>
                x.Id == id &&
                x.UserId == userId);

        if (notification is null)
        {
            return false;
        }

        _context.Notifications.Remove(notification);

        return _context.SaveChanges() > 0;
    }
}