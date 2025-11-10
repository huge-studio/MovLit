using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.UI;
using Oqtane.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Huge.MovLit.Services;
using Huge.MovLit.Enums;
using System;

namespace Huge.Dashboard
{
    public partial class Index : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public StoryService StoryService { get; set; }
        [Inject] public NavigationManager Nav { get; set; }

        private string _returnUrl;
        private string _settingsUrl;
        private bool _loading;
        protected bool _isBrowse;

        protected List<SectionConfig> Sections { get; set; } = new();
        protected bool ShowHero { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            _loading = true;
            _returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
            _settingsUrl = EditUrl("Settings", $"returnurl={_returnUrl}&tab=ModuleSettings");

            // detect browse mode from URL
            var uri = new Uri(Nav.Uri);
            _isBrowse = uri.AbsolutePath.Contains("browse", StringComparison.OrdinalIgnoreCase);

            if (!_isBrowse)
            {
                var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                var vm = new SettingsViewModel(SettingService, settings);
                Sections = vm.Sections ?? new();
                ShowHero = vm.ShowHero;
            }
            _loading = false;
        }
    }
}
