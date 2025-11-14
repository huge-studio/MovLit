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
    public partial class Browse : ModuleBase, IDisposable
    {
        [Inject] StoryService StoryService { get; set; }
        [Inject] NavigationManager Nav { get; set; }
        [Inject] IPageService PageService { get; set; }
        [Inject] TagFilterService TagFilter { get; set; }

        private List<Huge.MovLit.Models.Story> _stories = new();
        private List<Huge.MovLit.Models.Story> _viewStories = new();

        private bool _loading = true;
        private int _page = 1;
        private int _pageSize = 12;
        private bool _hasMore = false;
        private string _categorySlug = DashboardFilters.New;
        private string _search = string.Empty;
        private bool _filterByTag = false;
        private string _selectedTag = string.Empty;
        private bool _dataInitialized = false;
        private string _mode = DashboardFilters.Category;
        private bool _disposed;

        protected override async Task OnInitializedAsync()
        {
            TagFilter.TagChanged += OnExternalTagChanged;
            try
            {
                var (data, code) = await StoryService.GetAsync(DashboardFilters.FromSlug(_categorySlug), 0, ModuleState.ModuleId, all: true);
                _stories = data ?? new List<Huge.MovLit.Models.Story>();
                _dataInitialized = true;
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error loading stories", ex.Message);
            }
            ParseInitialQuery();
            BuildViewSlice();
        }

        private void OnExternalTagChanged(string? tag)
        {
            if (!_filterByTag) return;
            _selectedTag = tag ?? string.Empty;
            _page = 1;
            BuildViewSlice();
            StateHasChanged();
        }

        private void ParseInitialQuery()
        {
            var uri = new Uri(Nav.Uri);
            var qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var mode = qs.Get(DashboardFilters.Mode);
            _filterByTag = !string.IsNullOrWhiteSpace(mode) && mode.Equals(DashboardFilters.Tag, StringComparison.OrdinalIgnoreCase);
            _mode = _filterByTag ? DashboardFilters.Tag : DashboardFilters.Category;
            var cat = qs.Get(DashboardFilters.Category);
            if (!_filterByTag && !string.IsNullOrWhiteSpace(cat)) _categorySlug = cat;
            var tag = qs.Get(DashboardFilters.Tag);
            if (_filterByTag && !string.IsNullOrWhiteSpace(tag)) _selectedTag = tag; else if (!_filterByTag) _selectedTag = string.Empty;
            _page = 1;
        }

        private void OnModeChanged(ChangeEventArgs e)
        {
            var newMode = e.Value?.ToString() ?? DashboardFilters.Category;
            var wantTag = string.Equals(newMode, DashboardFilters.Tag, StringComparison.OrdinalIgnoreCase);
            _mode = newMode;
            _filterByTag = wantTag;
            if (!wantTag) _selectedTag = string.Empty;
            _page = 1;
            BuildViewSlice(); // no navigation, local re-filter
        }

        protected string CategorySlug
        {
            get => _categorySlug;
            set
            {
                if (_filterByTag) return;
                if (_categorySlug == value) return;
                _categorySlug = value ?? DashboardFilters.New;
                _page = 1;
                BuildViewSlice();
            }
        }

        protected string SelectedTag
        {
            get => _selectedTag;
            set
            {
                if (!_filterByTag) return;
                if (_selectedTag == value) return;
                _selectedTag = value ?? string.Empty;
                _page = 1;
                BuildViewSlice();
            }
        }

        protected string SearchTerm
        {
            get => _search;
            set
            {
                var newVal = value ?? string.Empty;
                if (_search == newVal) return;
                _search = newVal;
                _page = 1;
                BuildViewSlice();
            }
        }

        private void BuildViewSlice()
        {
            if (!_dataInitialized) return;
            IEnumerable<Huge.MovLit.Models.Story> working = _stories;

            if (!_filterByTag)
            {
                working = _categorySlug switch
                {
                    "Fresh" => working.OrderByDescending(s => s.CreatedOn),
                    "Top" => working.OrderByDescending(s => s.UpvoteCount),
                    "Trending" => working.Where(s => s.CreatedOn >= DateTime.UtcNow.AddDays(-30)).OrderByDescending(s => s.UpvoteCount).ThenByDescending(s => s.CreatedOn),
                    _ => working
                };
            }

            if (_filterByTag && !string.IsNullOrWhiteSpace(_selectedTag))
            {
                working = working.Where(s => s.Tags != null && s.Tags.Any(t => string.Equals(t, _selectedTag, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(_search))
            {
                var term = _search.ToLowerInvariant();
                working = working.Where(s => (s.Title?.ToLowerInvariant().Contains(term) ?? false)
                                          || (s.Description?.ToLowerInvariant().Contains(term) ?? false)
                                          || (s.Tags != null && s.Tags.Any(t => t.ToLowerInvariant().Contains(term))));
            }

            var list = working.ToList();
            _hasMore = list.Count > _page * _pageSize;
            _viewStories = list.Skip((_page - 1) * _pageSize).Take(_pageSize).ToList();
            _loading = false;
        }

        private Task PrevPage()
        {
            if (_page > 1)
            {
                _page--;
                BuildViewSlice();
            }
            return Task.CompletedTask;
        }

        private Task NextPage()
        {
            if (_hasMore)
            {
                _page++;
                BuildViewSlice();
            }
            return Task.CompletedTask;
        }

        private Task ClearSearch()
        {
            SearchTerm = string.Empty;
            return Task.CompletedTask;
        }

        private Task NavigateToStory(int pageId) => GoToStory(pageId);

        private async Task GoToStory(int pageId)
        {
            try
            {
                var page = await PageService.GetPageAsync(pageId);
                if (page != null) Nav.NavigateTo(page.Path);
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error navigating to page", ex.Message);
            }
        }

        private void GoBack()
        {
            var basePath = Nav.Uri.Split('?')[0];
            Nav.NavigateTo(basePath.Replace("/browse", "/dashboard"));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { TagFilter.TagChanged -= OnExternalTagChanged; } catch { }
        }
    }
}

