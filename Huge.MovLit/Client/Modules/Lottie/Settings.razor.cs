using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Models;
using Oqtane.Shared;


namespace Huge.Lottie
{
    public partial class Settings : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public IStringLocalizer<Settings> Localizer { get; set; }
        [Inject] public IFileService FileService { get; set; }

        public override string Title => "LottiePlayer Settings";

        private SettingsViewModel _settingsVM;
        private bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _settingsVM = new SettingsViewModel(SettingService, moduleSettings);
                _loading = false;
            }
            catch (Exception ex)
            {
                AddModuleMessage(ex.Message, MessageType.Error);
            }
        }

        public async Task UpdateSettings()
        {
            Dictionary<string, string> settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _settingsVM.SetSettings(SettingService, settings);
            await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);
        }
    }
}
