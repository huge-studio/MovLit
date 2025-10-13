using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Security;
using Oqtane.Shared;
using Huge.MovLit.Repository;
using System;

namespace Huge.MovLit.Services
{
    public partial class ServerStoryService
    {
        private MyModuleRepository _repo;

        public ServerStoryService(MyModuleRepository repo)
        {
            _repo = repo;
        }

        public Task<Dictionary<int, Models.StoryMetrics>> GetMetricsForStoriesAsync(int ModuleId, IEnumerable<int> storyIds)
        {
            var result = new Dictionary<int, Models.StoryMetrics>();

            var ids = storyIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0) return Task.FromResult(result);

            var views = _repo.GetStoryViewsForStories(ids);
            var likes = _repo.GetStoryLikesForStories(ids);

            foreach (var id in ids)
            {
                var vset = views.Where(v => v.StoryId == id).ToList();
                var lset = likes.Where(l => l.StoryId == id).ToList();

                result[id] = new Models.StoryMetrics
                {
                    StoryId = id,
                    ViewCount = vset.Count,
                    LikeCount = lset.Count,
                    VisitorViewIds = vset.Select(v => v.VisitorId).Distinct().ToList(),
                    VisitorLikeIds = lset.Select(l => l.VisitorId).Distinct().ToList(),
                    UserViewIds = vset.Select(v => (int?)v.UserId).Distinct().ToList(),
                    UserLikeIds = lset.Select(l => (int?)l.UserId).Distinct().ToList(),
                };
            }

            return Task.FromResult(result);
        }
    }
}
