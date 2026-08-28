namespace HrManagement.Api.Dtos.Notifications;

public record NotificationDto(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    bool IsRead,
    string Status,
    string ActionType
);

public record NotificationStatusRequest(string Status);
