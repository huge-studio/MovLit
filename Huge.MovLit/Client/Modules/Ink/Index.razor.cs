using Microsoft.AspNetCore.Components;
using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.Services;
using InkRun = global::Ink.Runtime;
using global::Ink;
using System.ComponentModel;
using System.Linq;
using Huge.MovLit.Models;

namespace Huge.Ink;

public partial class Index : ModuleBase, IDisposable
{

    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected ISettingService SettingService { get; set; }


    bool loading;

    // properties we listed for
    private const string UserStateProperty = "UserState";

    MarkupString _currentLine = new MarkupString();
    List<CustomInkChoice> _currentChoices = new();
    List<string> _inkState = new();

    bool _hasNext = false;
    bool _hasPrevious = false;
    bool _hasFinish = false;
    int _pageCount = 0;
    SettingsViewModel _settingsVM;

    protected InkRun.Story _story;
    private bool disposedValue;
    private string settingsUrl;
    private string returnUrl;

    protected override void OnInitialized()
    {
        // listen for changes to sitestate
        ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged += PropertyChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!ShouldRender()) return;

        loading = true;

        var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
        _settingsVM = new SettingsViewModel(SettingService, moduleSettings);

        returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
        settingsUrl = EditUrl("Settings", $"returnurl={returnUrl}&tab=ModuleSettings");

        CompileStory();

        if (PageState.EditMode || _story == null)
            return;

        try
        {
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

    protected void CompileStory()
    {

        try
        {
            if (string.IsNullOrEmpty(_settingsVM.Ink))
            {
                _story = null;
            }

            // add headers to the ink
            var headers = InkFunctions.GetHeaders();
            var ink = $"{headers}\n\n{_settingsVM.Ink}";

            // compile the story
            var compiler = new Compiler(ink);
            var compiledStory = compiler.Compile();
            _story = compiledStory;

            // bind external functions
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
        if (_story != null)
        {
            if (_story.canContinue)
            {
                string nextLine = _story.ContinueMaximally();
                _currentLine = ProcessStoryText(nextLine);
                _inkState.Add(_story.state.ToJson());
            }
            ProcessTags();
        }
    }

    private void Previous()
    {
        if (_story != null && _inkState.Any())
        {
            // remove the current state and load the last one
            _inkState.RemoveAt(_inkState.Count - 1); // could be a pop?

            var state = _inkState.LastOrDefault();
            _story.state.LoadJson(state);

            _currentLine = ProcessStoryText(_story.currentText);
            ProcessTags();
        }

    }

    MarkupString ProcessStoryText(string text)
    {
        // because the Ink authoring tool may not support "\n" line breaks, process "<br>" to linebreaks to allow Markdown to parse them
        text = text.Replace("\\", "\n");

        var output = Markdig.Markdown.ToHtml(text);

        return new MarkupString(output);
    }

    private void ProcessTags()
    {
        if (_story == null)
        {
            return;
        }

        if (_story.currentTags.Any())
        {

            var lottieUrl = ParseTagUrl(_story.currentTags, "lottie:");
            if (!string.IsNullOrEmpty(lottieUrl))
            {
                SiteState.Properties.Lottie = lottieUrl;
            }

            var imageUrl = ParseTagUrl(_story.currentTags, "image:");
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
        _hasPrevious = _inkState.Count > 1;
        _hasFinish = !_hasNext && _currentChoices.Count == 0;

        if (string.IsNullOrEmpty(_currentLine.Value) && !_hasNext && _currentChoices.Count == 0)
        {
            // if there's no more text and no more choices, we're at the end of the story
            return;
        }

        StateHasChanged();
    }

    private string ParseTagUrl(IEnumerable<string> tags, string prefix)
    {
        var tag = tags.FirstOrDefault(t => t.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        if (tag == null) return null;

        var value = tag.Substring(prefix.Length).Trim();

        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Case 1: explicit absolute URL
        if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        // Case 2: app-relative (~)
        if (value.StartsWith("~"))
        {
            // turn "~/files/..." into an absolute URL based on NavigationManager.BaseUri
            return NavigationManager.BaseUri.TrimEnd('/') + value[1..];
        }

        // Case 3: shorthand hostname (/myfolder/myimage.png
        return $"https://{value}";
    }


    async void PropertyChanged(object sender, PropertyChangedEventArgs e)
    {

        // listen for changes to siteState.Properties.InkVariable
        if (e.PropertyName == UserStateProperty)
        {
            // sync the user state with any ink variables
            InkFunctions.SyncUserState(_story, SiteState, PageState);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged -= PropertyChanged;
    }
}