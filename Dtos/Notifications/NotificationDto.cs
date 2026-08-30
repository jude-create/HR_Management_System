using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Notifications;

public record NotificationDto(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    bool IsRead,
    string Status,
    string ActionType
);

public record NotificationStatusRequest(
      [ Required, StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 500 characters.")]
    string Status);
