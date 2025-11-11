using Huge.MovLit.Enums;
using Huge.MovLit.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huge.Dashboard
{
    public partial class Browse : ModuleBase, IDisposable
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

        protected override void OnInitialized()
        {
            // react to querystring changes (eg. clicking tags in sidebar)
            Nav.LocationChanged += OnLocationChanged;
            base.OnInitialized();
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            _ = InvokeAsync(async () =>
            {
                ReadQuery(e.Location);
                await LoadAsync();
                await InvokeAsync(StateHasChanged);
            });
        }

        protected override async Task OnParametersSetAsync()
        {
            ReadQuery(Nav.Uri);
            await LoadAsync();
        }

        private void ReadQuery(string uriStr)
        {
            var uri = new Uri(uriStr);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            var mode = query.Get("mode");
            _filterByTag = !string.IsNullOrWhiteSpace(mode) && mode.Equals("tag", StringComparison.OrdinalIgnoreCase);

            var cval = query.Get("category");
            if (!string.IsNullOrWhiteSpace(cval)) _categorySlug = cval;

            var tval = query.Get("tag");
            if (!string.IsNullOrWhiteSpace(tval))
            {
                _filterByTag = true;
                _selectedTag = tval;
                _search = string.Empty; // don't mirror tag into search
            }
            else if (!_filterByTag)
            {
                // if not tag mode, keep previous selected tag cleared
                _selectedTag = string.Empty;
            }

            // reset to first page whenever the source parameters change
            _page = 1;
        }

        private async Task LoadAsync()
        {
            _loading = true;
            var useAll = _filterByTag || !string.IsNullOrWhiteSpace(_search);
            var filter = DashboardFilters.FromSlug(_categorySlug);
            var take = useAll ? 0 : (_pageSize * _page); // 0 => server returns all
            (var data, var code) = await StoryService.GetAsync(filter, take, ModuleState.ModuleId, all: useAll);
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

            // slice client-side
            _stories = all.Skip((_page - 1) * _pageSize).Take(_pageSize).ToList();
            _hasMore = all.Count > _page * _pageSize;
            _loading = false;
            StateHasChanged();
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

        public void Dispose()
        {
            try
            {
                Nav.LocationChanged -= OnLocationChanged;
            }
            catch (Exception ex)
            {
                logger.LogError("Error disposing location changed listener.", ex.Message);
           }
        }
    }
}
