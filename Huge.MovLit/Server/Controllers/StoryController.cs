using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Controllers;
using Huge.MovLit.Services;
using System.Threading.Tasks;
using Huge.MovLit.Repository;
using System;
using Huge.MovLit.Enums;
using System.Linq;

namespace Huge.MovLit.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class StoryController : ModuleControllerBase
    {
        private readonly MyModuleRepository _repo;
        private readonly ServerStoryService _service;

        public StoryController(MyModuleRepository repo, ServerStoryService service, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _repo = repo;
            _service = service;
        }

        // GET: api/<controller>/module?moduleid=x
        [HttpGet("module")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<ActionResult<Models.Story>> GetForModule([FromQuery] int moduleid)
        {
            try
            {
                if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
                {
                    _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Story GetByModule Attempt {ModuleId}", moduleid);
                    return Forbid();
                }

                var story = await _repo.GetStoryByModuleAsync(moduleid);
                if (story == null)
                {
                    return NotFound();
                }
                return Ok(story);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Read, "Error retrieving story for module {ModuleId}: {Message}", moduleid, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving story for module");
            }
        }

        // GET: api/<controller>?moduleid=x&filter=y&take=z
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<ActionResult<IEnumerable<Models.Story>>> Get(string filter = DashboardFilters.New, int take = 12)
        {
            try
            {
                IEnumerable<Models.Story> stories;

                if (filter == DashboardFilters.Top)
                {
                    stories = await _repo.GetTopAllTimeByLikesAsync(take);
                }
                else if (filter == DashboardFilters.Trending)
                {
                    DateTime nowUtc = DateTime.UtcNow;
                    stories = await _repo.GetTrendingAsync(take, nowUtc);
                }
                else // New
                {
                    stories = await _repo.GetNewestAsync(take);
                }

                //filter out incomplete stories (missing cover art) and the Ink Getting Started example story
                stories = stories.Where(s =>
                    s.CoverArtUrl != null &&
                    s.Title != InkGettingStarted.Title &&
                    s.Description != InkGettingStarted.Description);

                if (stories == null || !stories.Any())
                {
                    _logger.Log(LogLevel.Warning, this, LogFunction.Read, "No stories found");
                    return NotFound();
                }

                return Ok(stories);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Read, "Error retrieving stories: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving stories");
            }
        }


        // GET api/<controller>/5/123
        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.Story> Get(int id, int moduleid)
        {
            var story = await _repo.GetStoryAsync(id);
            if (story != null && IsAuthorizedEntityId(EntityNames.Module, story.ModuleId))
            {
                return story;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Story Get Attempt {StoryId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.Story> Post([FromBody] Models.Story story)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, story.ModuleId))
            {
                return await _repo.AddStoryAsync(story);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Story Post Attempt {Story}", story);
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.Story> Put(int id, [FromBody] Models.Story story)
        {
            if (ModelState.IsValid && story.StoryId == id && IsAuthorizedEntityId(EntityNames.Module, story.ModuleId))
            {
                return await _repo.UpdateStoryAsync(story);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Story Put Attempt {Story}", story);
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        // DELETE api/<controller>/5/123
        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            var story = await _repo.GetStoryAsync(id);
            if (story != null && IsAuthorizedEntityId(EntityNames.Module, story.ModuleId))
            {
                await _repo.DeleteStoryAsync(id);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Story Delete Attempt {StoryId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            }
        }

        // POST api/<controller>/metrics?moduleid=x
        [HttpPost("metrics")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Dictionary<int, Models.StoryMetrics>> Metrics([FromQuery] int moduleid, [FromBody] Dictionary<int, Models.StoryMetrics> payload)
        {
            if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Story Metrics Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }

            var ids = payload?.Keys?.Distinct().ToList() ?? new List<int>();
            var result = await _service.GetMetricsForStoriesAsync(moduleid, ids);

            // return as a dictionary keyed by StoryId (payload keys)
            return result;
        }

        // GET: api/<controller>/blog?moduleid=x&take=5&top=true
        [HttpGet("blog")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.BlogPost>> GetBlog(string moduleid, int take = 5)
        {
            if (!int.TryParse(moduleid, out var ModuleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Blog Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }

            return await _repo.GetRecentBlogPosts(take);
        }


        // POST api/<controller>/view-once
        [HttpPost("view")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IActionResult> AddView([FromQuery] int moduleid, [FromBody] Models.StoryView view)
        {
            if (!IsAuthorizedEntityId(EntityNames.Module, moduleid)) return Forbid();
            if (view == null || view.StoryId <= 0 || (!view.VisitorId.HasValue && !view.UserId.HasValue)) return BadRequest();

            // Record a view for each component initialization. Do not de-duplicate by user/visitor here.
            await _repo.AddStoryViewAsync(view);
            return Ok();
        }

        // POST api/<controller>/toggle-like
        [HttpPost("toggle-like")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<ActionResult<Models.StoryLike>> ToggleLike([FromQuery] int moduleid, [FromBody] Models.StoryLike like)
        {
            if (!IsAuthorizedEntityId(EntityNames.Module, moduleid)) return Forbid();
            if (like == null)
                return BadRequest("Missing like object.");
            if (like.StoryId <= 0)
                return BadRequest("Invalid StoryId.");
            if (!like.VisitorId.HasValue && !like.UserId.HasValue)
                return BadRequest("Missing user identification (VisitorId or UserId required).");

            var result = await _repo.ToggleStoryLikeAsync(like);
            return Ok(result);
        }
    }
}
