using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Notifications;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

// NotificationService handles the app's alert and notification feed.
public interface INotificationService
{
    IReadOnlyList<NotificationDto> GetNotifications();
    NotificationDto? UpdateNotificationStatus(Guid id, NotificationStatusRequest request);
    bool DeleteNotification(Guid id);
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

    public IReadOnlyList<NotificationDto> GetNotifications()
        // Newest notifications first, so the UI shows the latest events at the top.
    {
        var notifications = _context.Notifications.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToList();
        return _mapper.Map<List<NotificationDto>>(notifications);
    }

    public NotificationDto? UpdateNotificationStatus(Guid id, NotificationStatusRequest request)
    {
        // Convert the text status from the request into the enum used by the entity.
        var notification = _context.Notifications.FirstOrDefault(x => x.Id == id);
        if (notification is null || !Enum.TryParse<NotificationStatus>(request.Status, true, out var status))
        {
            return null;
        }

        notification.Status = status;
        _context.SaveChanges();
        return _mapper.Map<NotificationDto>(notification);
    }

    public bool DeleteNotification(Guid id)
    {
        // Notifications can be removed directly from the in-memory list.
        var notification = _context.Notifications.FirstOrDefault(x => x.Id == id);
        if (notification is null)
        {
            return false;
        }

        _context.Notifications.Remove(notification);
        return _context.SaveChanges() > 0;
    }
}
