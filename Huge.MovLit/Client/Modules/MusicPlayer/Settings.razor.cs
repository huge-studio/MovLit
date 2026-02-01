using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Huge.MusicPlayer
{
    public partial class Settings : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }

        public override string Title => "Music Player Settings";

        private string _trackUrl = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _trackUrl = SettingService.GetSetting(settings, "TrackUrl", "");
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Music Player Settings");
                AddModuleMessage("Error Loading Settings", MessageType.Error);
            }
        }

        public async Task UpdateSettings()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            SettingService.SetSetting(settings, "TrackUrl", _trackUrl);
            await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);
        }
    }
}
