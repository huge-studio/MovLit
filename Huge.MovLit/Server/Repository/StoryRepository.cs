using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Modules;
using System.Threading.Tasks;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace Huge.MovLit.Repository
{
    public partial class MyModuleRepository
    {
        public async Task<IEnumerable<Models.Story>> GetStoriesAsync()
        {
            using var db = _factory.CreateDbContext();
            return await db.Story.AsNoTracking().ToListAsync();
        }

        public async Task<Models.Story> GetStoryAsync(int StoryId, bool tracking = true)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return await db.Story.FindAsync(StoryId);
            }
            else
            {
                return await db.Story.AsNoTracking().FirstOrDefaultAsync(s => s.StoryId == StoryId);
            }
        }

        public async Task<Models.Story> AddStoryAsync(Models.Story story)
        {
            using var db = _factory.CreateDbContext();
            db.Story.Add(story);
            await db.SaveChangesAsync();
            return story;
        }

        public async Task DeleteStoryAsync(int StoryId)
        {
            using var db = _factory.CreateDbContext();
            var entity = db.Story.Find(StoryId);
            if (entity != null)
            {
                db.Story.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Models.Story> UpdateStoryAsync(Models.Story story)
        {
            using var db = _factory.CreateDbContext();
            var toUpdate = await GetStoryAsync(story.StoryId, tracking: true);

            db.Entry(toUpdate).CurrentValues.SetValues(story);
            db.Entry(toUpdate).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return toUpdate;
        }


        public async Task<IEnumerable<Models.Story>> GetTopAllTimeByLikesAsync(int take)
        {
            using var db = _factory.CreateDbContext();

            List<Models.Story> stories = await db.Story.AsNoTracking()
                .OrderByDescending(s => s.UpvoteCount)
                .ThenByDescending(s => s.CreatedOn)
                .Take(take)
                .ToListAsync();

            return stories;
        }

        public async Task<IEnumerable<Models.Story>> GetTrendingAsync(int take, DateTime nowUtc)
        {
            var since = nowUtc.AddDays(-30);

            using var db = _factory.CreateDbContext();

            var result = await db.Story.AsNoTracking()
                .Select(s => new
                {
                    Story = s,
                    Views30d = db.StoryView.Count(v => v.StoryId == s.StoryId && v.CreatedOn >= since)
                })
                .Where(x => x.Views30d > 0)                     // skip stories with no recent views
                .OrderByDescending(x => x.Views30d)
                .ThenByDescending(x => x.Story.CreatedOn)       // tie breaker
                .Take(take)
                .Select(x => x.Story)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Models.Story>> GetNewestAsync(int take)
        {

            using var db = _factory.CreateDbContext();
            List<Models.Story> stories = await db.Story.AsNoTracking()
                .OrderByDescending(s => s.CreatedOn)
                .Take(take)
                .ToListAsync();
            return stories;
        }

        public async Task<IEnumerable<Models.StoryView>> GetStoryViewsAsync(int StoryId)
        {
            using var db = _factory.CreateDbContext();
            return await db.StoryView.AsNoTracking().Where(v => v.StoryId == StoryId).ToListAsync();
        }

        public async Task<Models.StoryView> AddStoryViewAsync(Models.StoryView view)
        {
            using var db = _factory.CreateDbContext();
            db.StoryView.Add(view);
            await db.SaveChangesAsync();
            return view;
        }

        public async Task<IEnumerable<Models.StoryLike>> GetStoryLikesAsync(int StoryId)
        {
            using var db = _factory.CreateDbContext();
            return await db.StoryLike.AsNoTracking().Where(l => l.StoryId == StoryId).ToListAsync();
        }

        public async Task<Models.StoryLike> AddStoryLikeAsync(Models.StoryLike like)
        {
            using var db = _factory.CreateDbContext();
            db.StoryLike.Add(like);
            await db.SaveChangesAsync();
            return like;
        }

        public IEnumerable<Models.StoryView> GetStoryViewsForStories(IEnumerable<int> storyIds)
        {
            using var db = _factory.CreateDbContext();
            var ids = storyIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0) return Enumerable.Empty<Models.StoryView>();
            return db.StoryView.AsNoTracking().Where(v => ids.Contains(v.StoryId)).ToList();
        }

        public IEnumerable<Models.StoryLike> GetStoryLikesForStories(IEnumerable<int> storyIds)
        {
            using var db = _factory.CreateDbContext();
            var ids = storyIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0) return Enumerable.Empty<Models.StoryLike>();
            return db.StoryLike.AsNoTracking().Where(l => ids.Contains(l.StoryId)).ToList();
        }

        public async Task<IEnumerable<Models.BlogPost>> GetRecentBlogPosts(int take)
        {
            using var db = _factory.CreateDbContext();
            var blogs = await db.Blog.AsNoTracking()
                .OrderByDescending(b => b.CreatedOn)
                .Take(take)
                .ToListAsync();
            return blogs;
        }
    }
}
