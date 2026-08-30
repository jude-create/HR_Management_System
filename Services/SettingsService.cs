using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Settings;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// Thrown when the demo "current user" record is missing from the database.
// This should never happen in a correctly seeded system, so it's treated as
// an exceptional case rather than a normal validation failure.
public sealed class CurrentUserNotFoundException : Exception
{
    public CurrentUserNotFoundException()
        : base("The current user could not be found. The database may not be seeded correctly.")
    {
    }
}

// SettingsService manages the current user's preferences.
public interface ISettingsService
{
    SettingsDto GetSettings();
    SettingsDto UpdateSettings(SettingsUpdateRequest request);
}

// Settings are treated as personal preferences, not business records.
public sealed class SettingsService : ISettingsService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SettingsService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public SettingsDto GetSettings()
    {
        var user = _context.Users.Include(x => x.Settings).FirstOrDefault(x => x.Id == _context.CurrentUserId);
        if (user is null)
        {
            throw new CurrentUserNotFoundException();
        }

        return _mapper.Map<SettingsDto>(user.Settings);
    }

    public SettingsDto UpdateSettings(SettingsUpdateRequest request)
    {
        var user = _context.Users.Include(x => x.Settings).FirstOrDefault(x => x.Id == _context.CurrentUserId);
        if (user is null)
        {
            throw new CurrentUserNotFoundException();
        }

        if (!string.IsNullOrWhiteSpace(request.Appearance) &&
            Enum.TryParse<AppearanceMode>(request.Appearance, true, out var appearance))
        {
            user.Settings.Appearance = appearance;
        }

        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            user.Settings.Language = request.Language.Trim();
        }

        if (request.TwoFactor is not null)
        {
            user.Settings.TwoFactorEnabled = request.TwoFactor.Value;
        }

        if (request.MobilePush is not null)
        {
            user.Settings.MobilePushEnabled = request.MobilePush.Value;
        }

        if (request.DesktopNotifications is not null)
        {
            user.Settings.DesktopNotificationsEnabled = request.DesktopNotifications.Value;
        }

        if (request.EmailNotifications is not null)
        {
            user.Settings.EmailNotificationsEnabled = request.EmailNotifications.Value;
        }

        _context.SaveChanges();
        return _mapper.Map<SettingsDto>(user.Settings);
    }
}