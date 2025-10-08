using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Huge.MovLit.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Huge.Dashboard
{
    public partial class FromBlog : ModuleBase
    {
        [Inject] public StoryService StoryService { get; set; }
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        private List<Huge.MovLit.Models.BlogPost> _posts = new();
        private int _take = 6;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                var vm = new SettingsViewModel(SettingService, settings);
                _take = vm.BlogTake > 0 ? vm.BlogTake : 6;

                (var data, var code) = await StoryService.GetBlogAsync(ModuleState.ModuleId, _take);
                _posts = data ?? new();
            }
            catch (System.Exception ex)
            {
                await logger.LogError(ex, "Error Loading Blog Posts {Error}", ex.Message);
            }
        }

        private void Navigate(Huge.MovLit.Models.BlogPost post)
        {
            if (post == null || string.IsNullOrEmpty(post.Slug)) return;
            var path = post.Slug.StartsWith("/") ? post.Slug : $"/blog/!/{SiteState.Alias.AliasId}/{post.Slug}";
            Navigation.NavigateTo(path);
        }
    }
}
