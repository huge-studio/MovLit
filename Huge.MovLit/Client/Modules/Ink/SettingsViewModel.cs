using Oqtane.Services;
using System.Collections.Generic;

namespace Huge.Ink
{

    internal class SettingsViewModel
    {
        public SettingsViewModel(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            Ink = settingService.GetSetting(moduleSettings, nameof(Ink), Ink);

            string str = settingService.GetSetting(moduleSettings, nameof(CenterJustify), CenterJustify.ToString());
            bool centerJustify;
            if (bool.TryParse(str, out centerJustify))
            {
                CenterJustify = centerJustify;
            }
            str = settingService.GetSetting(moduleSettings, nameof(HasPrevious), HasPrevious.ToString());
            bool hasPrevious;
            if (bool.TryParse(str, out hasPrevious))
            {
                HasPrevious = hasPrevious;
            }
            ContinueText = settingService.GetSetting(moduleSettings, nameof(ContinueText), ContinueText);
        }

        public bool CenterJustify { get; set; } = true;
        public bool HasPrevious { get; set; } = false;
        public string Ink { get; set; } = string.Empty;
        public string ContinueText { get; set; } = "Continue";

        public void SetSettings(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {

            settingService.SetSetting(moduleSettings, nameof(Ink), Ink);
            settingService.SetSetting(moduleSettings, nameof(CenterJustify), CenterJustify.ToString());
            settingService.SetSetting(moduleSettings, nameof(ContinueText), ContinueText);
            settingService.SetSetting(moduleSettings, nameof(HasPrevious), HasPrevious.ToString());

        }
    }
}
