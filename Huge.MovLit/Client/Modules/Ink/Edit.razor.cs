using Huge.MovLit.Enums;
using Huge.MovLit.Services;
using Ink;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Oqtane.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Huge.Ink
{
    public partial class Edit : ModuleBase
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public StoryService StoryService { get; set; }
        [Inject] public IFileService FileService { get; set; }

        public override SecurityAccessLevel SecurityAccessLevel => SecurityAccessLevel.Edit;

        public override string Actions => "Add,Edit";

        public override string Title => "Manage Story";

        private Huge.MovLit.Models.Story _story;
        private bool _editorReady;
        private bool _valueApplied;

        private ElementReference _inkTextArea;

        private string _returnUrl;
        private string _errorMessage;
        private string[] _selectedTags = Array.Empty<string>();



        private readonly List<string> _allTags = StoryTags.GetAllTags;


        protected override void OnInitialized()
        {
            // parse query
            var uri = new Uri(NavigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            PageState.ReturnUrl = query.Get("returnUrl") ?? "/";
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!ShouldRender()) return;
            try
            {
                (_story, var code) = await StoryService.GetForModuleAsync(ModuleState.ModuleId);
                if (_story is null || code != HttpStatusCode.OK)
                {
                    throw new Exception($"Story returned null for edit action");
                }
                _selectedTags = _story.Tags?.ToArray() ?? Array.Empty<string>();

            }
            catch (Exception ex)
            {
                await logger.LogError($"Error retrieving Story: {ex.Message}");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!string.IsNullOrEmpty(_inkTextArea.Id))
            {
                if (!_editorReady)
                {
                    _editorReady = true;
                    await JSRuntime.InvokeVoidAsync("inkEditor.init", _inkTextArea, new { lineNumbers = true });
                }
                if (_editorReady && _story is not null && !_valueApplied)
                {
                    _valueApplied = true;
                    await JSRuntime.InvokeVoidAsync("inkEditor.setValue", _inkTextArea, _story.InkJson ?? string.Empty);
                }
            }
        }

        private async Task OnCoverSelected(int fileId)
        {
            try
            {
                var file = await FileService.GetFileAsync(fileId);
                if (_story != null)
                {
                    _story.CoverArtUrl = file?.Url;
                }
            }
            catch
            {
                await logger.LogError($"Error retrieving file {fileId} for cover art selection");
            }
        }


        private async Task Save()
        {
            try
            {
                if (_story == null)
                {
                    AddModuleMessage("No story exists for this module. Visit the Ink module once to create the starter story, then return to edit action.", MessageType.Warning);
                    return;
                }
                // pull latest content from CodeMirror (if active)
                try
                {
                    var current = await JSRuntime.InvokeAsync<string>("inkEditor.getValue", _inkTextArea);
                    // Always assign so clearing the editor persists as empty string
                    _story.InkJson = current ?? string.Empty;
                }
                catch (Exception ex)
                {
                    await logger.LogError($"Error retrieving Ink JSON from editor: {ex.Message}");
                }
                if (!string.IsNullOrWhiteSpace(_story.InkJson))
                {
                    if (!CanCompileStory())
                    {
                        return;
                    }
                }

                if (_selectedTags != null && _selectedTags.Length > 0)
                {
                    _story.Tags = _selectedTags.ToList();
                }
                else
                {
                    _story.Tags = new List<string>();
                }

                if (_story.StoryId > 0)
                {
                    await StoryService.UpdateAsync(_story);
                }
                else
                {
                    var (created, code) = await StoryService.AddAsync(_story);
                    if (created != null) _story = created;
                }

                AddModuleMessage("Saved Story", MessageType.Success);
                var url = NavigateUrl(PageState.ReturnUrl ?? string.Empty);
                NavigationManager.NavigateTo(url);
            }
            catch (Exception ex)
            {
                AddModuleMessage(ex.Message, MessageType.Error);
            }
        }

        private void Cancel()
        {
            var url = NavigateUrl(PageState.ReturnUrl ?? string.Empty);
            NavigationManager.NavigateTo(url);
        }

        protected bool CanCompileStory()
        {
            _errorMessage = string.Empty;

            try
            {
                // add headers to the ink
                var headers = InkFunctions.GetHeaders();
                var ink = $"{headers}\n\n{_story?.InkJson}";

                if (string.IsNullOrWhiteSpace(_story?.InkJson))
                {
                    _errorMessage = "Compilation failed: No story generated.";
                    StateHasChanged();
                    return false;
                }

                // compile the story
                var compiler = new Compiler(ink);
                var compiledStory = compiler.Compile();
                return compiledStory != null;
            }
            catch (Exception ex)
            {
                _errorMessage = Regex.Replace(
                                ex.Message,
                                @"(?i)\bline\s+(\d+)",
                                m => $"line {Math.Max(1, int.Parse(m.Groups[1].Value) - 8)}");
                StateHasChanged();
                return false;
            }
        }
    }
}
