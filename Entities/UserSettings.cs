namespace HR_Management_System.Entities;

// Per-user preferences. NOT a business record like Employee/Payroll -
// it's config, so it lives 1-to-1 with User rather than as its own list of "settings".
public class UserSettings
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public AppearanceMode Appearance { get; set; } = AppearanceMode.System;
    public string Language { get; set; } = "en";
    public bool TwoFactorEnabled { get; set; }
    public bool MobilePushEnabled { get; set; } = true;
    public bool DesktopNotificationsEnabled { get; set; } = true;
    public bool EmailNotificationsEnabled { get; set; } = true;
}
