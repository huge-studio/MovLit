using Huge.MovLit.Enums;
using Huge.MovLit.Services;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models = Huge.MovLit.Models;

namespace Huge.Dashboard
{
    public partial class ContentSection : ModuleBase
    {
        [Inject] public StoryService StoryService { get; set; }
        [Inject] public IPageService PageService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Parameter] public string SectionTitle { get; set; } = DashboardFilters.New;
        [Parameter] public string? ActionText { get; set; } = "View More";
        [Parameter] public int Cards { get; set; } = 9;
        [Parameter] public bool Horizontal { get; set; } = false;

        private List<Models.Story> _stories;
        protected Dictionary<int, Models.StoryMetrics> _metrics = new();

        protected override async Task OnParametersSetAsync()
        {
            if (!ShouldRender()) return;
            try
            {
                (_stories, var code) = await StoryService.GetAsync(SectionTitle, Cards, ModuleState.ModuleId);
                if (_stories?.Count > 0)
                {
                    var ids = new List<int>(_stories.Count);
                    foreach (var s in _stories) 
                    {
                        ids.Add(s.StoryId); 
                    }
                    (var metrics, var mstatus) = await StoryService.GetMetricsAsync(ModuleState.ModuleId, ids);
                    _metrics = metrics ?? new();
                }
                else
                {
                    _metrics = new();
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error retrieving stories", ex.Message);
            }
        }

        public async Task NavigateToPage(int pageId)
        {
            try
            {
                var page = await PageService.GetPageAsync(pageId);
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

        private void NavigateToBrowse()
        {
            // Add browse query param to current page path and carry category slug
            var slug = DashboardFilters.ToSlug(SectionTitle);
            var basePath = new Uri(NavigationManager.Uri).GetLeftPart(UriPartial.Path);
            NavigationManager.NavigateTo($"{basePath}?browse=true&category={Uri.EscapeDataString(slug)}");
        }
    }
}
