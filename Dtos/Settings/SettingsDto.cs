namespace HrManagement.Api.Dtos.Settings;

// No Role/Permissions here - that's identity data, it belongs on AuthUserDto
// and is changed via an admin action, not a "settings" toggle.
public record SettingsDto(
    string Appearance,
    string Language,
    bool TwoFactor,
    bool MobilePush,
    bool DesktopNotifications,
    bool EmailNotifications
);

public record SettingsUpdateRequest(
    string? Appearance,
    string? Language,
    bool? TwoFactor,
    bool? MobilePush,
    bool? DesktopNotifications,
    bool? EmailNotifications
);
