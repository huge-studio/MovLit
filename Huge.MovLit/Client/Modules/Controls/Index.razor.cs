using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Huge.MovLit.Enums;
using Huge.MovLit.Models;
using Huge.MovLit.Services;

namespace Huge.Controls
{
    public partial class Index : ModuleBase, IDisposable
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public StoryService StoryService { get; set; }

        // Audio/Fullscreen state
        private string _currentTrack;
        private bool _isPlaying = false;
        private bool _isLooping = true;
        private bool _isMuted = false;
        private bool _isFullscreen = false;

        // Metrics state
        private int _viewCount = 0;
        private int _likeCount = 0;
        private bool _liked = false;
        private bool _canLike = false;
        private bool _viewSent = false;
        private Story _storyEntity;

        private const string StoryPropertyName = "Story";

        private string _settingsUrl;

        private ElementReference _audioPlayer;
        private IJSObjectReference _module;

        protected override void OnInitialized()
        {
            ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged += PropertyChanged;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!ShouldRender()) return;

            try
            {
                var returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
                _settingsUrl = EditUrl("Settings", $"returnurl={returnUrl}");

                await LoadSettings();
                
                // Check URL for fullscreen parameter using PageState.QueryString
                CheckFullscreenFromUrl();

                // Metrics: check if user can like
                _canLike = PageState.VisitorId > 0 || (PageState.User?.UserId ?? 0) > 0;

                // Try to get story from SiteState (set by Ink module)
                if (SiteState.Properties.Story is Story story)
                {
                    _storyEntity = story;
                    await LoadMetricsAsync();
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Controls {Error}", ex.Message);
                if (!PageState.EditMode)
                {
                    AddModuleMessage("Error Loading Controls", MessageType.Error);
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Modules/Huge.Controls/Module.js");
                
                // Apply fullscreen state from URL on initial load
                if (_isFullscreen)
                {
                    await _module.InvokeVoidAsync("setFullscreen", true);
                }
                
                if (!string.IsNullOrEmpty(_currentTrack))
                {
                    await _module.InvokeVoidAsync("setLoop", _audioPlayer, _isLooping);
                    
                    if (_isPlaying)
                    {
                        await _module.InvokeVoidAsync("play", _audioPlayer);
                    }
                }

                // Log view on first render
                if (!_viewSent && _storyEntity?.StoryId > 0)
                {
                    await LogViewAsync();
                    await LoadMetricsAsync();
                    StateHasChanged();
                }
            }
        }

        private void CheckFullscreenFromUrl()
        {
            // Use PageState.QueryString dictionary for reading query parameters
            if (PageState.QueryString.ContainsKey("fullscreen"))
            {
                var fullscreenValue = PageState.QueryString["fullscreen"];
                _isFullscreen = string.Equals(fullscreenValue, "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        private async Task LoadSettings()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _currentTrack = SettingService.GetSetting(settings, "TrackUrl", "");
        }

        private async Task ToggleFullscreen()
        {
            _isFullscreen = !_isFullscreen;
            
            if (_module != null)
            {
                await _module.InvokeVoidAsync("setFullscreen", _isFullscreen);
            }
            
            // Update the URL with the new fullscreen state
            UpdateUrlWithFullscreen();
            
            await InvokeAsync(StateHasChanged);
        }

        private void UpdateUrlWithFullscreen()
        {
            // Build new URL using PageState.Uri base path
            var baseUri = PageState.Uri.GetLeftPart(UriPartial.Path);
            
            // Build query string from PageState.QueryString, updating fullscreen
            var queryParams = new System.Collections.Generic.List<string>();
            foreach (var kvp in PageState.QueryString)
            {
                if (!string.Equals(kvp.Key, "fullscreen", StringComparison.OrdinalIgnoreCase))
                {
                    queryParams.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            queryParams.Add($"fullscreen={_isFullscreen.ToString().ToLower()}");
            
            var queryString = string.Join("&", queryParams);
            var newUri = string.IsNullOrEmpty(queryString) ? baseUri : $"{baseUri}?{queryString}";
            
            NavigationManager.NavigateTo(newUri, forceLoad: false, replace: true);
        }

        private async Task Play()
        {
            _isPlaying = true;
            if (_module != null)
            {
                await _module.InvokeVoidAsync("play", _audioPlayer);
            }
            await InvokeAsync(StateHasChanged);
        }

        private async Task Pause()
        {
            _isPlaying = false;
            if (_module != null)
            {
                await _module.InvokeVoidAsync("pause", _audioPlayer);
            }
            await InvokeAsync(StateHasChanged);
        }

        private async Task ToggleMute()
        {
            _isMuted = !_isMuted;
            if (_module != null)
            {
                await _module.InvokeVoidAsync("setMuted", _audioPlayer, _isMuted);
            }
            await InvokeAsync(StateHasChanged);
        }

        #region Metrics

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

        #endregion

        public void Dispose()
        {
            ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged -= PropertyChanged;
            _module?.DisposeAsync();
        }

        async void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == NotifyPropertyName.SoundUrl)
            {
                var soundUrl = (string)SiteState.Properties[NotifyPropertyName.SoundUrl];
                if (!PageState.EditMode && !string.IsNullOrWhiteSpace(soundUrl))
                {
                    _currentTrack = soundUrl;
                    _isPlaying = true;
                    
                    await InvokeAsync(StateHasChanged);
                    
                    if (_module != null)
                    {
                        await _module.InvokeVoidAsync("setLoop", _audioPlayer, _isLooping);
                        await _module.InvokeVoidAsync("play", _audioPlayer);
                    }
                }
            }
            else if (e.PropertyName == StoryPropertyName)
            {
                if (SiteState.Properties.Story is Story story)
                {
                    _storyEntity = story;
                    _viewSent = false;
                    await LoadMetricsAsync();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
    }
}
