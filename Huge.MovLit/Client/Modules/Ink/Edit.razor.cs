using Huge.MovLit.Enums;
using Huge.MovLit.Services;
using Ink;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.UI;
using Oqtane.Services;
using System;
using System.Collections.Generic;
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



        private ElementReference form;
        private bool validated = false;

        private int _id;
        private string _name;
        private string _createdby;
        private DateTime _createdon;
        private string _modifiedby;
        private DateTime _modifiedon;

        // Form state
        private string _title;
        private string _description;
        private string _coverArtUrl;
        private string _inkJson;
        private string _returnUrl;
        private string _errorMessage;

        private List<string> _selectedTags = new();
        private readonly List<string> _allTags = StoryTags.GetAllTags;


        protected override async Task OnInitializedAsync()
        {
            var uri = new Uri(NavigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            PageState.ReturnUrl = query.Get("returnUrl") ?? "/";

            // Load existing story for this module (if any)
            (_story, var code) = await StoryService.GetForModuleAsync(ModuleState.ModuleId);

            // hydrate form
            if (_story != null)
            {
                _title = _story.Title;
                _description = _story.Description;
                _coverArtUrl = _story.CoverArtUrl;
                _inkJson = _story.InkJson;
            }
            else
            {
                _title = string.Empty;
                _description = string.Empty;
                _coverArtUrl = string.Empty;
                _inkJson = string.Empty;
            }
        }

        private async Task OnCoverSelected(int fileId)
        {
            try
            {
                var file = await FileService.GetFileAsync(fileId);
                _coverArtUrl = file?.Url;
            }
            catch
            {
                _coverArtUrl = $"/Files/{fileId}";
            }
        }

        private void OnTagsChanged(ChangeEventArgs e)
        {
            // Handle multi-select: collect selected <option> values
            // In real binding, we'd parse from e.Value; here keep existing selection
        }

        private async Task Save()
        {
            try
            {
                if (_story == null)
                {
                    AddModuleMessage("No story exists for this module. Visit the Ink module once to create the starter story, then return to edit.", MessageType.Warning);
                    return;
                }
                // Map form to entity
                _story.Title = _title;
                _story.Description = _description;
                _story.CoverArtUrl = _coverArtUrl;
                _story.InkJson = _inkJson;

                if (!string.IsNullOrWhiteSpace(_inkJson))
                {
                    if (!CanCompileStory())
                    {
                        return;
                    }
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
                var ink = $"{headers}\n\n{_inkJson}";

                if (ink == string.Empty)
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
                _errorMessage = ex.Message;
                StateHasChanged();
                return false;
            }
        }
    }
}
