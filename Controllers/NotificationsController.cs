using System.Security.Claims;
using HR_Management_System.Dtos.Notifications;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers;

// NotificationsController returns and updates notification records.
[Authorize]
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
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Forbid();
        }

        return Ok(_notificationService.GetNotifications(userId));
    }

    // Updates the read/unread/archived status.
    [HttpPut("{id:guid}/status")]
    public ActionResult<NotificationDto> UpdateStatus(
        Guid id,
        [FromBody] NotificationStatusRequest request)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Forbid();
        }

        var notification =
            _notificationService.UpdateNotificationStatus(
                id,
                userId,
                request);

        return notification is null
            ? BadRequest("Invalid notification status or notification not found.")
            : Ok(notification);
    }

    // Deletes a notification.
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteNotification(Guid id)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Forbid();
        }

        var deleted =
            _notificationService.DeleteNotification(id, userId);

        return deleted
            ? NoContent()
            : NotFound("Notification not found.");
    }
}