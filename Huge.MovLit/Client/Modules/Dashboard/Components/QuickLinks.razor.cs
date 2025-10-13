using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Huge.Dashboard
{
    public partial class QuickLinks : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        private List<QuickLink> _links = new();

        protected override async Task OnInitializedAsync()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            var vm = new SettingsViewModel(SettingService, settings);
            _links = vm.QuickLinks ?? new();
        }

        private async Task Navigate(QuickLink link)
        {
            if (link == null || string.IsNullOrWhiteSpace(link.Url)) return;
            if (link.IsExternal)
            {
                await JSRuntime.InvokeVoidAsync("open", link.Url, "_blank");
            }
            else
            {
                var path = link.Url.StartsWith("/") ? link.Url : $"/{link.Url}";
                Navigation.NavigateTo(path);
            }
        }
    }
}
