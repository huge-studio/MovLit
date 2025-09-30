using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Models;
using Oqtane.Shared;
using Huge.MoveLit.Enums;


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
        private string _sourceType = NotifyPropertyName.Lottie;
        private bool _useLottieUrl = false;
        private bool _useImageUrl = false;

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

        private async Task HandleUpload(int fileId)
        {
            try
            {
                var uploadedFile = await FileService.GetFileAsync(fileId);

                if (uploadedFile != null)
                {
                    if (_sourceType == NotifyPropertyName.Lottie)
                    {
                        _settingsVM.LottieSource = uploadedFile.Url;
                        _settingsVM.ImgSource = null;
                    }
                    else
                    {
                        _settingsVM.ImgSource = uploadedFile.Url;
                        _settingsVM.LottieSource = null;
                    }
                }
                else
                {
                    AddModuleMessage("Error uploading file", MessageType.Error);
                }
            }
            catch (Exception ex)
            {
                AddModuleMessage(ex.Message, MessageType.Error);
                await logger.LogError(ex, "Error Uploading File {Error}", ex.Message);
            }
        }


        public async Task UpdateSettings()
        {
            Dictionary<string, string> settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _settingsVM.SetSettings(SettingService, settings);
            await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);
        }

        public void SetSourceType(string type)
        {
            _sourceType = type;

            if (type == "lottie")
            {
                _settingsVM.ImgSource = null;
            }
            else
            {
                _settingsVM.LottieSource = null;
            }
        }
    }
}

