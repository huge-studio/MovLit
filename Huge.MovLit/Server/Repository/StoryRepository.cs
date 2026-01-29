using Huge.MovLit.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using Oqtane.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huge.MovLit.Repository
{
    public partial class MyModuleRepository
    {
        public async Task<Models.Story> GetStoryByModuleAsync(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return await db.Story.AsNoTracking().FirstOrDefaultAsync(s => s.ModuleId == moduleId);
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
                .Where(s =>
                    s.CoverArtUrl != null &&
                    s.Title != InkGettingStarted.Title &&
                    s.Description != InkGettingStarted.Description)
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
                .Where(x =>
                    x.Views30d > 0 &&                   // skip stories with no recent views
                    x.Story.CoverArtUrl != null &&
                    x.Story.Title != InkGettingStarted.Title &&
                    x.Story.Description != InkGettingStarted.Description)
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
                .Where(s =>
                    s.CoverArtUrl != null &&
                    s.Title != InkGettingStarted.Title &&
                    s.Description != InkGettingStarted.Description)
                .OrderByDescending(s => s.CreatedOn)
                .Take(take)
                .ToListAsync();
            return stories;
        }

        public async Task<Models.StoryView> AddStoryViewAsync(Models.StoryView view)
        {
            using var db = _factory.CreateDbContext();
            db.StoryView.Add(view);
            await db.SaveChangesAsync();
            return view;
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

        public async Task<Models.StoryLike> ToggleStoryLikeAsync(Models.StoryLike like)
        {
            using var db = _factory.CreateDbContext();
            var existing = await db.StoryLike
                .FirstOrDefaultAsync(l => l.StoryId == like.StoryId && (l.UserId == like.UserId || (like.VisitorId.HasValue && l.VisitorId == like.VisitorId)));

            var story = await db.Story.FirstOrDefaultAsync(s => s.StoryId == like.StoryId);
            if (story == null) return null;

            if (existing != null)
            {
                db.StoryLike.Remove(existing);
                if (story.UpvoteCount > 0) story.UpvoteCount -= 1;
            }
            else
            {
                await db.StoryLike.AddAsync(like);
                story.UpvoteCount += 1;
            }
            await db.SaveChangesAsync();
            return existing ?? like;
        }

        public async Task<IEnumerable<Models.Story>> GetAllValidStoriesAsync(int? take = null)
        {
            using var db = _factory.CreateDbContext();
            var query = db.Story.AsNoTracking().Where(s =>
                s.CoverArtUrl != null &&
                s.Title != InkGettingStarted.Title &&
                s.Description != InkGettingStarted.Description);

            query = query.OrderByDescending(s => s.CreatedOn);
            if (take.HasValue && take.Value > 0)
            {
                query = query.Take(take.Value);
            }
            return await query.ToListAsync();
        }
    }
}
