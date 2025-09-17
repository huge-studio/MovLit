using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Huge.MoveLit.Enums;

namespace Huge.Lottie;

public partial class Index : ModuleBase, IDisposable
{
    [Inject] public ISettingService SettingService { get; set; }

    bool loading = true;

    private string _lottieSource = string.Empty;
    private string _imageSource = string.Empty;

    private string _returnUrl;
    private string _settingsUrl;

    ElementReference _lottieElem;
    private bool _needsPlay;

    protected override void OnInitialized()
    {
        ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged += PropertyChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!ShouldRender() || !loading) return;

        try
        {
            _returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
            _settingsUrl = EditUrl("Settings", $"returnurl={_returnUrl}&tab=ModuleSettings");

            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            var vm = new SettingsViewModel(SettingService, settings);

            _lottieSource = vm.LottieSource;
            _imageSource = vm.ImgSource;
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading Content {Error}", ex.Message);
            if (!PageState.EditMode)
            {
                AddModuleMessage("Error Loading Content", MessageType.Error);
            }
        }
        finally
        {
            loading = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_needsPlay && !string.IsNullOrWhiteSpace(_lottieSource))
        {
            _needsPlay = false;
            await JSRuntime.InvokeVoidAsync("playWhenReady", _lottieElem);
        }
    }

    public void Dispose()
    {
        ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged -= PropertyChanged;
    }

    void PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == NotifyPropertyName.Lottie)
        {
            var src = (string)SiteState.Properties[NotifyPropertyName.Lottie];
            if (!PageState.EditMode && !string.IsNullOrWhiteSpace(src))
            {
                _lottieSource = src;
                _needsPlay = true;
                StateHasChanged();
            }
        }
        if (e.PropertyName == NotifyPropertyName.Image)
        {
            var src = (string)SiteState.Properties[NotifyPropertyName.Image];
            if (!PageState.EditMode && !string.IsNullOrWhiteSpace(src))
            {
                _imageSource = src;
                StateHasChanged();
            }
        }
    }
}

