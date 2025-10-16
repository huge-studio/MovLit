using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Ink;
using Oqtane.Interfaces;


namespace Huge.Ink
{
    public partial class Settings : ModuleBase, ISettingsControl
    {

        public override string Title => "Ink Settings";

        bool loading;
        protected string _errorMessage;
        SettingsViewModel _settingsVM;

        [Inject] ISettingService SettingService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            loading = true;

            try
            {
                var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _settingsVM = new SettingsViewModel(SettingService, moduleSettings);

            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading settings {Error}", ex.Message);
            }

            loading = false;
        }

        protected override void OnParametersSet()
        {
            if (!ShouldRender()) return;

            var uri = new Uri(NavigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            PageState.ReturnUrl = query.Get("returnUrl") ?? "/";
        }

        public async Task UpdateSettings()
        {
            try
            {
                var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);

                _settingsVM.SetSettings(SettingService, settings);

                await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                AddModuleMessage(ex.Message, MessageType.Error);
            }
        }
    }
}