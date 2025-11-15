using Oqtane.Services;
using System.Collections.Generic;

namespace Huge.Uploader
{
    public class SettingsViewModel
    {
        public string FilePath = string.Empty;

        public SettingsViewModel(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            FilePath = settingService.GetSetting(moduleSettings, nameof(FilePath), FilePath);;
        }

        public void SetSettings(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            settingService.SetSetting(moduleSettings, nameof(FilePath), FilePath);
        }
    }
}
