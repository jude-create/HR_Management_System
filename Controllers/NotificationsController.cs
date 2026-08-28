using HrManagement.Api.Dtos.Notifications;
using HrManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// NotificationsController returns and updates notification records.
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // Gets the latest notifications first.
    [HttpGet]
    public ActionResult<IReadOnlyList<NotificationDto>> GetNotifications()
        => Ok(_notificationService.GetNotifications());

    // Updates the read/unread/archived status.
    [HttpPut("{id:guid}/status")]
    public ActionResult<NotificationDto> UpdateStatus(Guid id, [FromBody] NotificationStatusRequest request)
    {
        var notification = _notificationService.UpdateNotificationStatus(id, request);
        return notification is null ? BadRequest("Invalid notification status or notification not found.") : Ok(notification);
    }

    // Deletes a notification.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteNotification(Guid id)
    {
        var deleted = _notificationService.DeleteNotification(id);
        return deleted ? NoContent() : NotFound("Notification not found.");
    }
}
