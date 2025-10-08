using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Huge.MovLit.Services;
using System.Threading.Tasks;
using System;

namespace Huge.Dashboard
{
    public partial class Hero : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public StoryService StoryService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IPageService PageService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected MovLit.Models.Story _story;
        protected bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            var vm = new SettingsViewModel(SettingService, settings);
            if (vm.ShowHero && vm.FeaturedStoryId.HasValue)
            {
                try
                {
                    (var data, var status) = await StoryService.GetByIdAsync(vm.FeaturedStoryId.Value, ModuleState.ModuleId);
                    _story = data;
                }
                catch (Exception)
                {
                    _story = null;
                }
            }
            _loading = false;
        }

        protected async Task NavigateToFeatured()
        {
            try
            {
                var page = await PageService.GetPageAsync(_story.PageId);
                if (page != null)
                {
                    NavigationManager.NavigateTo(page.Path);
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error navigating to page", ex.Message);
            }
        }
    }
}
