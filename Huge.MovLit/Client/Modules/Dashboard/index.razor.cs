using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.UI;
using Oqtane.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Huge.MovLit.Services;
using Huge.MovLit.Enums;

namespace Huge.Dashboard
{
    public partial class Index : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public StoryService StoryService { get; set; }

        private string _returnUrl;
        private string _settingsUrl;
        private bool _loading;

        protected List<SectionConfig> Sections { get; set; } = new();
        protected bool ShowHero { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            _loading = true;
            _returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
            _settingsUrl = EditUrl("Settings", $"returnurl={_returnUrl}&tab=ModuleSettings");

            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            var vm = new SettingsViewModel(SettingService, settings);
            Sections = vm.Sections ?? new();
            ShowHero = vm.ShowHero;
            _loading = false;
        }
    }
}
