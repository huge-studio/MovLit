using global::Ink;
using Huge.MovLit.Models;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using InkRun = global::Ink.Runtime;

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
    private string _settingsUrl;
    private string _returnUrl;
    private string _editUrl;

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

        _returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
        _settingsUrl = EditUrl("Settings", $"returnurl={_returnUrl}&tab=ModuleSettings");
        _editUrl = EditUrl("Edit");


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
        if(_story == null) return;

        //If using the lottie initial source, set it in the ink variable only once
        if (_story.variablesState.GlobalVariableExistsWithName("initialUrl") && string.IsNullOrWhiteSpace(_story.variablesState["initialUrl"] as string))
        {
            SetInitialUrl();
        }

        if (_story.canContinue)
        {
            //string nextLine = _story.ContinueMaximally();
            //_currentLine = ProcessStoryText(nextLine);
            //_inkState.Add(_story.state.ToJson());

            var allTags = new List<string>();
            var allLines = new List<string>();

            while (_story.canContinue)
            {
                allLines.Add(_story.Continue());
                allTags.AddRange(_story.currentTags);
            }
            _currentLine = ProcessStoryText(string.Concat(allLines));
            _inkState.Add(_story.state.ToJson());
        }

        ProcessTags();
    }

    public void SetInitialUrl()
    {
        var lottie = SiteState.Properties.Lottie;
        var image = SiteState.Properties.Image;

        // pick the first non-empty source
        var source = !string.IsNullOrWhiteSpace(lottie) ? lottie
                   : !string.IsNullOrWhiteSpace(image) ? image
                   : null;

        // only set if we have a source and the Ink var is blank
        if (!string.IsNullOrWhiteSpace(source))
        {
            _story.variablesState["initialUrl"] = source;
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

        //
        if (_story.currentTags.Any(s => s.Contains("lottie", StringComparison.OrdinalIgnoreCase) || s.Contains("image", StringComparison.OrdinalIgnoreCase)))
        {

            var lottieUrl = UrlParser.ParseTagUrl(_story.currentTags, "lottie:", NavigationManager);
            if (!string.IsNullOrEmpty(lottieUrl))
            {
                SiteState.Properties.Lottie = lottieUrl;
            }

            var imageUrl = UrlParser.ParseTagUrl(_story.currentTags, "image:", NavigationManager);
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