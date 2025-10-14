using Oqtane.Services;
using System.Collections.Generic;

namespace Huge.Ink
{

    internal class SettingsViewModel
    {
        public SettingsViewModel(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            string str = settingService.GetSetting(moduleSettings, nameof(CenterJustify), CenterJustify.ToString());
            bool centerJustify;
            if (bool.TryParse(str, out centerJustify))
            {
                CenterJustify = centerJustify;
            }
            // support legacy or alternate key names
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
        public string ContinueText { get; set; } = "Continue";

        public void SetSettings(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            settingService.SetSetting(moduleSettings, nameof(CenterJustify), CenterJustify.ToString());
            settingService.SetSetting(moduleSettings, nameof(ContinueText), ContinueText);
            settingService.SetSetting(moduleSettings, nameof(HasPrevious), HasPrevious.ToString());

        }
    }
}
