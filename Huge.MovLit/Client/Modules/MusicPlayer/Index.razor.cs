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

namespace Huge.MusicPlayer
{
    public partial class Index : ModuleBase, IDisposable
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private string _currentTrack;
        private bool _isPlaying = false;
        private bool _isLooping = true;
        private bool _isMuted = false;

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
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Music Player {Error}", ex.Message);
                if (!PageState.EditMode)
                {
                    AddModuleMessage("Error Loading Music Player", MessageType.Error);
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Modules/Huge.MusicPlayer/Module.js");
                
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

        private async Task LoadSettings()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _currentTrack = SettingService.GetSetting(settings, "TrackUrl", "");
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
