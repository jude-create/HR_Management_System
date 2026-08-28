using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Settings;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

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
        // Read the current user's saved settings from the store.
        var user = _context.Users.Include(x => x.Settings).FirstOrDefault(x => x.Id == _context.CurrentUserId);
        return user is null
            ? new SettingsDto("System", "en", false, true, true, true)
            : _mapper.Map<SettingsDto>(user.Settings);
    }

    public SettingsDto UpdateSettings(SettingsUpdateRequest request)
    {
        // Update only the fields the caller supplied.
        var user = _context.Users.Include(x => x.Settings).FirstOrDefault(x => x.Id == _context.CurrentUserId);
        if (user is null)
        {
            return new SettingsDto("System", "en", false, true, true, true);
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
