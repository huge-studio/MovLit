using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Huge.MovLit.Enums;

namespace Huge.Controls
{
    public partial class Index : ModuleBase, IDisposable
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private string _currentTrack;
        private bool _isPlaying = false;
        private bool _isLooping = true;
        private bool _isMuted = false;
        private bool _isFullscreen = false;

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
                
                // Check URL for fullscreen parameter
                CheckFullscreenFromUrl();
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
            }
        }

        private void CheckFullscreenFromUrl()
        {
            var uri = new Uri(NavigationManager.Uri);
            var qs = HttpUtility.ParseQueryString(uri.Query);
            var fullscreenValue = qs.Get("fullscreen");
            
            if (!string.IsNullOrWhiteSpace(fullscreenValue))
            {
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
            var uri = new Uri(NavigationManager.Uri);
            var baseUri = uri.GetLeftPart(UriPartial.Path);
            var qs = HttpUtility.ParseQueryString(uri.Query);
            
            // Update fullscreen parameter
            qs["fullscreen"] = _isFullscreen.ToString().ToLower();
            
            var queryString = qs.ToString();
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
        }
    }
}
