using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Huge.Uploader
{
    public partial class Settings : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public IFileService FileService { get; set; }

        public override string Title => "Uploader Settings";

        private SettingsViewModel _vm;
        private bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _vm = new SettingsViewModel(SettingService, moduleSettings);
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
                    _vm.FilePath = uploadedFile.Url;
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
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _vm.SetSettings(SettingService, settings);
            await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);
        }
    }
}
