using HR_Management_System.Dtos.Settings;

namespace HR_Management_System.Services.Settings
{
    public interface ISettingsService
    {
        SettingsDto GetSettings();
        SettingsDto UpdateSettings(SettingsUpdateRequest request);
    }

}
