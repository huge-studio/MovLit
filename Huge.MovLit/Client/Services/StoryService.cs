using Huge.MovLit.Enums;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Huge.MovLit.Services
{
    public class StoryService : ResponseServiceBase
    {
        public StoryService(IHttpClientFactory factory, SiteState siteState) : base(factory, siteState) { }

        private string Apiurl => CreateApiUrl("Story");

        public async Task<(List<Models.Story> Data, HttpStatusCode Code)> GetAsync(string filter, int take, int moduleId, bool all = false)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}?filter={Uri.EscapeDataString(filter)}&take={take}&all={(all ? "true" : "false")}", EntityNames.Module, moduleId);
            (var data, var response) = await GetJsonWithResponseAsync<List<Models.Story>>(url);
            return (data, response.StatusCode);
        }

        // Get the story for a module (if any)
        public async Task<(Models.Story, HttpStatusCode)> GetForModuleAsync(int ModuleId)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/module?moduleid={ModuleId}", EntityNames.Module, ModuleId);
            (var data, var response) = await GetJsonWithResponseAsync<Models.Story>(url);
            return (data, response.StatusCode);
        }

        // Single item by id
        public async Task<(Models.Story, HttpStatusCode)> GetByIdAsync(int StoryId, int ModuleId)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{StoryId}/{ModuleId}", EntityNames.Module, ModuleId);
            (var data, var response) = await GetJsonWithResponseAsync<Models.Story>(url);
            return (data, response.StatusCode);
        }

        // Create
        public async Task<(Models.Story, HttpStatusCode)> AddAsync(Models.Story story)
        {
            if (story == null)
            {
                return (null, HttpStatusCode.NotFound);
            }
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, story.ModuleId);
            (var data, var response) = await PostJsonWithResponseAsync(url, story);
            return (data, response.StatusCode);
        }

        // Update
        public async Task<(Models.Story, HttpStatusCode)> UpdateAsync(Models.Story story)
        {
            if (story == null)
            {
                return (null, HttpStatusCode.NotFound);
            }
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{story.StoryId}", EntityNames.Module, story.ModuleId);
            (var data, var response) = await PutJsonWithResponseAsync(url, story);
            return (data, response.StatusCode);
        }

        // Delete
        public async Task<HttpStatusCode> DeleteAsync(int StoryId, int ModuleId)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{StoryId}/{ModuleId}", EntityNames.Module, ModuleId);
            var response = await DeleteWithResponseAsync<object>(url);
            return response.StatusCode;
        }

        public async Task<(Dictionary<int, Models.StoryMetrics>, HttpStatusCode)> GetMetricsAsync(int ModuleId, IEnumerable<int> storyIds)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/metrics?moduleid={ModuleId}", EntityNames.Module, ModuleId);
            var ids = storyIds?.Distinct().ToList() ?? new List<int>();

            // keys = storyIds, values ignored by server
            var body = ids.ToDictionary(id => id, _ => (Models.StoryMetrics)null);

            (var data, var response) = await PostJsonWithResponseAsync(url, body);
            return (data, response.StatusCode);
        }

        public async Task<(List<Models.BlogPost>, HttpStatusCode)> GetBlogAsync(int ModuleId, int take)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/blog?moduleid={ModuleId}&take={take}", EntityNames.Module, ModuleId);
            (var data, var response) = await GetJsonWithResponseAsync<List<Models.BlogPost>>(url);
            return (data, response.StatusCode);
        }

        public async Task<HttpStatusCode> AddView(int ModuleId, Models.StoryView view)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/view?moduleid={ModuleId}", EntityNames.Module, ModuleId);
            (var data, var response) = await PostJsonWithResponseAsync(url, view);
            return response.StatusCode;
        }

        public async Task<(Models.StoryLike, HttpStatusCode)> ToggleLikeAsync(int ModuleId, Models.StoryLike like)
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/toggle-like?moduleid={ModuleId}", EntityNames.Module, ModuleId);
            (var data, var response) = await PostJsonWithResponseAsync(url, like);
            return (data, response.StatusCode);
        }
    }
}
