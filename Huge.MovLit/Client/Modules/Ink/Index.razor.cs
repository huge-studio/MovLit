using global::Ink;
using Huge.MovLit.Models;
using Huge.MovLit.Services;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InkRun = global::Ink.Runtime;
using Huge.MovLit.Enums;

namespace Huge.Ink
{
    public partial class Index : ModuleBase, IDisposable
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ISettingService SettingService { get; set; }
        [Inject] protected StoryService StoryService { get; set; }

        private bool loading = true;

        private const string UserStateProperty = "UserState";

        private MarkupString _currentLine = new MarkupString();
        private List<CustomInkChoice> _currentChoices = new();
        private List<string> _inkState = new();

        private bool _hasNext = false;
        private bool _hasPrevious = false;
        private bool _hasFinish = false;
        private int _pageCount = 0;
        private SettingsViewModel _settingsVM;
        protected Story _storyEntity;
        private bool _storyLoaded = false;
        private bool _viewSent = false;
        private int _viewCount = 0;
        private int _likeCount = 0;
        private bool _liked = false;
        private bool _canLike = false;

        protected InkRun.Story _story;
        private bool disposedValue;
        private string _settingsUrl;
        private string _returnUrl;
        private string _editUrl;

        private readonly List<StepSnapshot> _history = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged += PropertyChanged;

                _returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
                _settingsUrl = EditUrl("Settings", $"returnurl={_returnUrl}&tab=ModuleSettings");

                await EnsureStoryLoadedAsync();
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Content {Error}", ex.Message);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!ShouldRender()) return;

            loading = true;

            try
            {
                var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _settingsVM = new SettingsViewModel(SettingService, moduleSettings);
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Settings {Error}", ex.Message);
            }

            try
            {
                await EnsureStoryLoadedAsync();
                CompileStory();
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error loading or compiling story {Error}", ex.Message);
            }

            if (_story == null)
            {
                loading = false;
                return;
            }

            try
            {
                _canLike = PageState.VisitorId > 0 || (PageState.User?.UserId ?? 0) > 0;

                // metrics after potential view logged in OnInitializedAsync
                await LoadMetricsAsync();

                _inkState = new();
                if (_story.canContinue)
                {
                    Next();
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Content {Error}", ex.Message);
            }

            loading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;
            try
            {
                if (!_viewSent)
                {
                    await EnsureStoryLoadedAsync();
                    await LogViewAsync();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error logging view {Error}", ex.Message);
            }
        }


        private async Task EnsureStoryLoadedAsync()
        {
            try
            {
                if (_storyLoaded && _storyEntity != null) return;

                (_storyEntity, var code) = await StoryService.GetForModuleAsync(ModuleState.ModuleId);

                if (_storyEntity is null || code == HttpStatusCode.NotFound)
                {
                    // Create the starter story on first visit
                    var starter = new Story
                    {
                        ModuleId = ModuleState.ModuleId,
                        PageId = ModuleState.PageId,
                        Title = InkGettingStarted.Title,
                        Description = InkGettingStarted.Description,
                        AuthorName = PageState.User.Username,
                        AuthorId = PageState.User.UserId,
                        InkJson = InkGettingStarted.Body
                    };
                    var (created, addCode) = await StoryService.AddAsync(starter);
                    if (addCode == HttpStatusCode.OK || addCode == HttpStatusCode.Created)
                    {
                        _storyEntity = created;
                    }
                    else
                    {
                        _storyEntity = new Story();
                    }
                    _editUrl = EditUrl("Edit", $"?returnurl={_returnUrl}&edit=true");
                }
                else
                {
                    _editUrl = EditUrl("Edit", $"?returnurl={_returnUrl}&edit=true");
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error loading story for module {Error}", ex.Message);
            }
            finally
            {
                _storyLoaded = true;

            }
        }

        private async Task LogViewAsync()
        {
            if (_storyEntity?.StoryId <= 0) return;
            // Only skip logging if both visitor and user are missing
            if (PageState.VisitorId <= 0 && (PageState.User?.UserId ?? 0) <= 0) return;

            var view = new StoryView
            {
                StoryId = _storyEntity.StoryId,
                VisitorId = PageState.VisitorId > 0 ? PageState.VisitorId : null,
                UserId = (PageState.User?.UserId ?? 0) > 0 ? PageState.User.UserId : null,
            };
            await StoryService.AddView(ModuleState.ModuleId, view);
            _viewSent = true;
        }

        private async Task ToggleLikeAsync()
        {
            if (!_canLike || _storyEntity?.StoryId <= 0) return;

            var like = new StoryLike
            {
                StoryId = _storyEntity.StoryId,
                VisitorId = PageState.VisitorId > 0 ? PageState.VisitorId : null,
                UserId = (PageState.User?.UserId ?? 0) > 0 ? PageState.User.UserId : (int?)null,
            };
            var (result, code) = await StoryService.ToggleLikeAsync(ModuleState.ModuleId, like);
            if (code == HttpStatusCode.OK)
            {
                _liked = !_liked;
                _likeCount += _liked ? 1 : -1;
            }
        }

        private async Task LoadMetricsAsync()
        {
            if (_storyEntity?.StoryId <= 0) return;
            var ids = new[] { _storyEntity.StoryId };
            var (data, code) = await StoryService.GetMetricsAsync(ModuleState.ModuleId, ids);
            if (code == HttpStatusCode.OK && data != null && data.TryGetValue(_storyEntity.StoryId, out var m))
            {
                _viewCount = m.ViewCount;
                _likeCount = m.LikeCount;
                var visitorId = PageState.VisitorId > 0 ? (int?)PageState.VisitorId : null;
                var userId = (PageState.User?.UserId ?? 0) > 0 ? (int?)PageState.User.UserId : null;
                _liked = (visitorId.HasValue && (m.VisitorLikeIds?.Contains(visitorId) ?? false))
                         || (userId.HasValue && (m.UserLikeIds?.Contains(userId) ?? false));
            }
        }

        protected void CompileStory()
        {
            try
            {
                if (_storyEntity == null || string.IsNullOrEmpty(_storyEntity.InkJson))
                {
                    _story = null;
                    return;
                }

                var headers = InkFunctions.GetHeaders();
                var ink = $"{headers}\n\n{_storyEntity?.InkJson}";

                var compiler = new Compiler(ink);
                var compiledStory = compiler.Compile();
                _story = compiledStory;

                InkFunctions.BindExternalFunctions(_story, SiteState, this, NavigationManager);
            }
            catch (Exception ex)
            {
                _story = null;
                logger.LogError(ex, "Error Loading story {message}", ex.Message);
                AddModuleMessage("Error Loading Story", MessageType.Error);
            }
        }

        protected void ChoiceSelected(CustomInkChoice choice)
        {
            var choiceIndex = choice.Index;
            _story.ChooseChoiceIndex(choiceIndex);
            Next();
        }

        private void Next()
        {
            if (_story == null) return;

            // If available set initialUrl from lottie module
            InkVariables.SetInitialUrl(_story, SiteState);
            // If available set player_name from user name
            InkVariables.SetPlayerName(_story, PageState);

            var allTags = new List<string>();
            var allLines = new List<string>();

            if (_story.canContinue)
            {

                while (_story.canContinue)
                {
                    allLines.Add(_story.Continue());
                    if (_story.currentTags is { Count: > 0 }) allTags.AddRange(_story.currentTags);
                }
                _currentLine = ProcessStoryText(string.Concat(allLines));
                _inkState.Add(_story.state.ToJson());
            }

            ProcessTags(allTags);

            _history.Add(new StepSnapshot
            {
                StateJson = _story.state.ToJson(),
                CurrentLine = string.Concat(allLines),
                LottieUrl = SiteState.Properties.Lottie,
                ImageUrl = SiteState.Properties.Image
            });
        }

        public void SetInitialUrl()
        {
            var lottie = SiteState.Properties.Lottie;
            var image = SiteState.Properties.Image;

            var source = !string.IsNullOrWhiteSpace(lottie) ? lottie
                       : !string.IsNullOrWhiteSpace(image) ? image
                       : null;

            if (!string.IsNullOrWhiteSpace(source))
            {
                _story.variablesState["initialUrl"] = source;
            }
        }

        private void Previous()
        {
            if (_story == null) return;
            if (_history.Count <= 1) return; // already at the first step

            // Pop current step from both stacks (keep them in lockstep)
            _history.RemoveAt(_history.Count - 1);
            _inkState.RemoveAt(_inkState.Count - 1);

            // Snapshot to restore
            var snap = _history[^1];

            // Restore Ink state
            var stateJson = _inkState.LastOrDefault();
            if (!string.IsNullOrEmpty(stateJson))
            {
                _story.state.LoadJson(stateJson);
            }

            // Reset the SiteState properties for the Lottie Module
            SiteState.Properties.Lottie = snap.LottieUrl;
            SiteState.Properties.Image = snap.ImageUrl;

            //Set the current line
            _currentLine = ProcessStoryText(snap.CurrentLine);
            _hasPrevious = _history.Count > 1 ? true : false;

        }


        MarkupString ProcessStoryText(string text)
        {
            text = text.Replace("\\", "\n");
            var output = Markdig.Markdown.ToHtml(text);
            return new MarkupString(output);
        }

        private void ProcessTags(List<string> tags)
        {
            if (_story == null)
            {
                return;
            }

            if (tags.Any(s => s.Contains("lottie", StringComparison.OrdinalIgnoreCase) || s.Contains("image", StringComparison.OrdinalIgnoreCase)))
            {
                var lottieUrl = UrlParser.ParseTagUrl(tags, "lottie:", NavigationManager);
                if (!string.IsNullOrEmpty(lottieUrl))
                {
                    SiteState.Properties.Lottie = lottieUrl;
                }

                var imageUrl = UrlParser.ParseTagUrl(tags, "image:", NavigationManager);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    SiteState.Properties.Image = imageUrl;
                }
            }

            _currentChoices = _story.currentChoices
                                    .Select(choice => new CustomInkChoice
                                    {
                                        Text = choice.text,
                                        Tags = choice.tags,
                                        Index = choice.index,
                                        PathStringOnChoice = choice.pathStringOnChoice
                                    })
                                    .ToList();

            _hasNext = _story.canContinue;
            _hasPrevious = _inkState.Count > 1 && _settingsVM.HasPrevious;
            _hasFinish = !_hasNext && _currentChoices.Count == 0;

            if (string.IsNullOrEmpty(_currentLine.Value) && !_hasNext && _currentChoices.Count == 0)
            {
                return;
            }

            StateHasChanged();
        }

        async void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == UserStateProperty)
            {
                InkFunctions.SyncUserState(_story, SiteState, PageState);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged -= PropertyChanged;
        }
    }

    public class StepSnapshot
    {
        public string StateJson { get; init; }
        public string CurrentLine { get; init; }
        public string LottieUrl { get; init; }
        public string ImageUrl { get; init; }
    }
}