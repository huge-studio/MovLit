using Huge.MovLit.Enums;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huge.Dashboard
{
    public partial class TagsList : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Parameter] public int Count { get; set; } = 8;
        [Parameter] public bool ShowBrowseAllLink { get; set; } = true;

        protected List<string> _tags = new();

        protected override async Task OnInitializedAsync()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            var vm = new SettingsViewModel(SettingService, settings);
            _tags = (vm.PopularTags != null && vm.PopularTags.Count > 0)
                ? vm.PopularTags
                : StoryTags.GetAllTags;
        }

        private void GoTag(string tag)
        {
            NavigationManager.NavigateTo($"/dashboard/browse?tag={Uri.EscapeDataString(tag)}");
        }

        private void GoAllTags()
        {
            NavigationManager.NavigateTo("/dashboard/browse?mode=tag");
        }
    }
}
