using Huge.MovLit.Enums;
using Huge.MovLit.Services;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huge.Dashboard
{
    public partial class Browse : ModuleBase
    {

        [Inject] StoryService StoryService { get; set; }
        [Inject] NavigationManager Nav { get; set; }

        [Inject] IPageService PageService { get; set; }

        private List<Huge.MovLit.Models.Story> _stories = new();
        private bool _loading = true;
        private int _page = 1;
        private int _pageSize = 12;
        private bool _hasMore = false;
        private string _categorySlug = "fresh"; // default
        private string _search = string.Empty;

        // new: filter mode and tag selection
        private bool _filterByTag = false;
        private string _selectedTag = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            // read query params manually
            var uri = new Uri(Nav.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var mode = query.Get("mode");
            if (!string.IsNullOrWhiteSpace(mode) && mode.Equals("tag", StringComparison.OrdinalIgnoreCase))
            {
                _filterByTag = true;
            }
            var cval = query.Get("category");
            if (!string.IsNullOrWhiteSpace(cval)) _categorySlug = cval;
            var tval = query.Get("tag");
            if (!string.IsNullOrWhiteSpace(tval))
            {
                _filterByTag = true;
                _selectedTag = tval;
                _search = string.Empty; // don't mirror tag into search
            }
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            _loading = true;
            var filter = DashboardFilters.FromSlug(_categorySlug);
            (var data, var code) = await StoryService.GetAsync(filter, _pageSize * _page, ModuleState.ModuleId);
            var all = data ?? new();

            if (_filterByTag && !string.IsNullOrWhiteSpace(_selectedTag))
            {
                all = all.Where(s => (s.Tags != null && s.Tags.Any(t => string.Equals(t, _selectedTag, StringComparison.OrdinalIgnoreCase)))).ToList();
            }

            if (!string.IsNullOrWhiteSpace(_search))
            {
                var term = _search.ToLowerInvariant();
                all = all.Where(s => (s.Title?.ToLowerInvariant().Contains(term) ?? false) || (s.Description?.ToLowerInvariant().Contains(term) ?? false) || (s.Tags != null && s.Tags.Any(t => t.ToLowerInvariant().Contains(term)))).ToList();
            }

            // slice
            _stories = all.Skip((_page - 1) * _pageSize).Take(_pageSize).ToList();
            _hasMore = all.Count > _page * _pageSize;
            _loading = false;
        }

        private async Task PrevPage()
        {
            if (_page > 1)
            {
                _page--;
                await LoadAsync();
            }
        }

        private async Task NextPage()
        {
            if (_hasMore)
            {
                _page++;
                await LoadAsync();
            }
        }

        private async Task OnSearchChange(ChangeEventArgs e)
        {
            _search = Convert.ToString(e?.Value) ?? string.Empty;
            _page = 1;
            await LoadAsync();
        }

        private async Task ClearSearch()
        {
            _search = string.Empty;
            _page = 1;
            await LoadAsync();
        }

        private async Task NavigateToStory(int pageId)
        {
            try
            {
                var page = await PageService.GetPageAsync(pageId);
                if (page != null)
                {
                    Nav.NavigateTo(page.Path);
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error navigating to page", ex.Message);
            }
        }

        private async Task OnModeChanged(ChangeEventArgs e)
        {
            _filterByTag = string.Equals(Convert.ToString(e?.Value), "tag", StringComparison.OrdinalIgnoreCase);
            _page = 1;
            await LoadAsync();
        }

        private async Task OnCategoryChanged(ChangeEventArgs e)
        {
            _categorySlug = Convert.ToString(e?.Value) ?? "fresh";
            _page = 1;
            await LoadAsync();
        }

        private async Task OnTagChanged(ChangeEventArgs e)
        {
            _selectedTag = Convert.ToString(e?.Value) ?? string.Empty;
            _page = 1;
            await LoadAsync();
        }

        private void GoBack()
        {
            // Navigate back to dashboard root without browse
            var basePath = Nav.Uri.Split('?')[0];
            // assume base dashboard path is everything up to '/browse'
            if (basePath.EndsWith("/browse", StringComparison.OrdinalIgnoreCase))
            {
                Nav.NavigateTo(basePath.Substring(0, basePath.Length - "/browse".Length));
            }
            else
            {
                Nav.NavigateTo("/dashboard");
            }
        }
    }
}
