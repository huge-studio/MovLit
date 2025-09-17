using Oqtane.Models;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huge.Lottie
{
    public class SettingsViewModel
    {
        public string LottieSource = string.Empty;
        public string ImgSource = string.Empty;

        public SettingsViewModel(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            LottieSource = settingService.GetSetting(moduleSettings, nameof(LottieSource), LottieSource);
            ImgSource = settingService.GetSetting(moduleSettings, nameof(ImgSource), ImgSource);
        }

        public string Value { get; set; } = string.Empty;

        public void SetSettings(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {

            settingService.SetSetting(moduleSettings, nameof(LottieSource), LottieSource);
            settingService.SetSetting(moduleSettings, nameof(ImgSource), ImgSource);
        }
    }
}
