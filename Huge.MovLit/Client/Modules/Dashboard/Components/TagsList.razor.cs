using Huge.MovLit.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Huge.MovLit.Services;

namespace Huge.Dashboard
{
    public partial class TagsList : ModuleBase, IDisposable
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public TagFilterService TagFilter { get; set; }

        [Parameter] public int Count { get; set; } = 8;
        [Parameter] public bool ShowBrowseAllLink { get; set; } = true;
        [Parameter] public bool DisableInteraction { get; set; } = false; // disable links when already browsing

        protected List<string> _tags = new();
        private string _basePath;
        private bool _isBrowseContext;
        private bool _disposed;

        protected override void OnInitialized()
        {
            // track URL changes so browse context updates immediately
            NavigationManager.LocationChanged += OnLocationChanged;
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            var vm = new SettingsViewModel(SettingService, settings);
            _tags = (vm.PopularTags != null && vm.PopularTags.Count > 0)
                ? vm.PopularTags
                : StoryTags.GetAllTags;
        }

        protected override void OnParametersSet()
        {
            UpdateContextFromUri(NavigationManager.Uri);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            UpdateContextFromUri(e.Location);
            _ = InvokeAsync(StateHasChanged);
        }

        private void UpdateContextFromUri(string uriStr)
        {
            var uri = new Uri(uriStr);
            _basePath = uri.GetLeftPart(UriPartial.Path);
            var qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
            _isBrowseContext = !string.IsNullOrWhiteSpace(qs.Get("browse"));
        }

        private void GoTag(string tag)
        {
            if (DisableInteraction || _isBrowseContext)
            {
                TagFilter.SetTag(tag);
                return;
            }
            NavigationManager.NavigateTo($"{_basePath}?browse=1&mode=tag&tag={Uri.EscapeDataString(tag)}");
        }

        private void GoAllTags()
        {
            if (DisableInteraction || _isBrowseContext)
            {
                TagFilter.SetTag(null);
                return;
            }
            NavigationManager.NavigateTo($"{_basePath}?browse=1&mode=tag");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { NavigationManager.LocationChanged -= OnLocationChanged; } catch { }
        }
    }
}
