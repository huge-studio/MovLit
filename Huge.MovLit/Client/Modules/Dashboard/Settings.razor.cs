using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huge.Dashboard
{
    public partial class Settings : ModuleBase
    {
        [Inject] public ISettingService SettingService { get; set; }
        [Inject] public IPageService PageService { get; set; }

        public override string Title => "Dashboard Settings";

        protected SettingsViewModel _vm = new();
        protected bool _loading = true;
        protected List<Oqtane.Models.Page> _pages = new();

        protected override async Task OnInitializedAsync()
        {
            var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _vm = new SettingsViewModel(SettingService, settings);

            try
            {
                var siteId = SiteState?.Alias?.SiteId ?? 0;
                _pages = siteId > 0 ? await PageService.GetPagesAsync(siteId) : new();
            }
            catch { _pages = new(); }

            _loading = false;
        }

        public async Task UpdateSettings()
        {
            try
            {
                var settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _vm.SetSettings(SettingService, settings);
                await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                AddModuleMessage(ex.Message, MessageType.Error);
            }
        }

        // Sections management 
        protected void AddSection() => _vm.Sections.Add(new SectionConfig());

        protected void RemoveSection(int index)
        {
            if (index >= 0 && index < _vm.Sections.Count)
            {
                _vm.Sections.RemoveAt(index);
            }
        }

        protected void MoveUp(int index)
        {
            if (index <= 0 || index >= _vm.Sections.Count) return;
            var item = _vm.Sections[index];
            _vm.Sections.RemoveAt(index);
            _vm.Sections.Insert(index - 1, item);
        }
        protected void MoveDown(int index)
        {
            if (index < 0 || index >= _vm.Sections.Count - 1) return;
            var item = _vm.Sections[index];
            _vm.Sections.RemoveAt(index);
            _vm.Sections.Insert(index + 1, item);
        }

        // Quick Links management
        protected void AddLink() => _vm.QuickLinks.Add(new QuickLink());
        protected void RemoveLink(int index)
        {
            if (index >= 0 && index < _vm.QuickLinks.Count)
            {
                _vm.QuickLinks.RemoveAt(index);
            }
        }
        protected void MoveLinkUp(int index)
        {
            if (index <= 0 || index >= _vm.QuickLinks.Count) return;
            var item = _vm.QuickLinks[index];
            _vm.QuickLinks.RemoveAt(index);
            _vm.QuickLinks.Insert(index - 1, item);
        }

        protected void MoveLinkDown(int index)
        {
            if (index < 0 || index >= _vm.QuickLinks.Count - 1) return;
            var item = _vm.QuickLinks[index];
            _vm.QuickLinks.RemoveAt(index);
            _vm.QuickLinks.Insert(index + 1, item);
        }

        protected void OnPageChanged(int index)
        {
            if (index < 0 || index >= _vm.QuickLinks.Count) return;
            var q = _vm.QuickLinks[index];
            if (q.PageId.HasValue)
            {
                var page = _pages?.FirstOrDefault(p => p.PageId == q.PageId.Value);
                if (page != null)
                {
                    q.Name = string.IsNullOrWhiteSpace(q.Name) ? page.Name : q.Name; // don't overwrite custom name if already set
                    q.Url = page.Path;
                }
            }
        }
        protected void ToggleTag(string tag, bool isChecked)
        {
            if (string.IsNullOrWhiteSpace(tag)) return;
            if (isChecked)
            {
                if (!_vm.PopularTags.Contains(tag)) _vm.PopularTags.Add(tag);
            }
            else
            {
                _vm.PopularTags.Remove(tag);
            }
        }
    }
}
