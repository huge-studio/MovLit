using Huge.MovLit.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Huge.Ink
{
    public partial class Edit : ModuleBase
    {
        [Inject] public NavigationManager NavigationManager { get; set; }

        public override SecurityAccessLevel SecurityAccessLevel => SecurityAccessLevel.Edit;

        public override string Actions => "Add,Edit";

        public override string Title => "Manage MyModule";

        public override bool UseAdminContainer => true;

     

        private ElementReference form;
        private bool validated = false;

        private int _id;
        private string _name;
        private string _createdby;
        private DateTime _createdon;
        private string _modifiedby;
        private DateTime _modifiedon;

        // Local form state (no binding to services yet)
        private string _title;
        private string _author;
        private string _description;
        private string _coverArtUrl;
        private string _inkJson;
        private string _continueText = "Continue";

        private List<string> _selectedTags = new();
        private readonly List<string> _allTags = new()
        {
            "Adventure","Fantasy","Sci-Fi","Mystery","Romance","Horror","Comedy","Action","Drama","Puzzle"
        };

        protected override async Task OnInitializedAsync()
        {
            // UI-only: seed defaults
            _inkJson = "VAR initialUrl = \"\"\n\n// Start of the story\n-> start";
        }

        private async Task OnCoverSelected(int fileId)
        {
            // UI-only: simulate resolving a URL from a file id
            _coverArtUrl = $"/Files/{fileId}";
            await Task.Yield();
        }

        private void OnTagsChanged(ChangeEventArgs e)
        {
            // Handle multi-select: collect selected <option> values
            // In real binding, we'd parse from e.Value; here keep existing selection
        }

        private Task Save()
        {
            // UI stub: later this will call StoryService Add/Update
            AddModuleMessage("Saved (UI stub)", MessageType.Success);
            return Task.CompletedTask;
        }

        private void Cancel()
        {
            var url = NavigateUrl(PageState.ReturnUrl ?? string.Empty);
            NavigationManager.NavigateTo(url);
        }
    }
}
