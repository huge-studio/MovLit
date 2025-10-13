using System.Collections.Generic;
using System.Threading.Tasks;

namespace Huge.Dashboard
{
    public partial class Sidebar
    {
        private List<QuickLink> _links = new();

        protected override async Task OnInitializedAsync()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            var vm = new SettingsViewModel(SettingService, settings);
            _links = vm.QuickLinks ?? new();
        }

        private void Navigate(QuickLink link)
        {
            if (link == null || string.IsNullOrWhiteSpace(link.Url)) return;
            if (link.IsExternal)
            {
                Navigation.NavigateTo(link.Url, forceLoad: true);
            }
            else
            {
                var path = link.Url.StartsWith("/") ? link.Url : $"/{link.Url}";
                Navigation.NavigateTo(path);
            }
        }
    }
}
