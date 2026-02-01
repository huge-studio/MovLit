using Huge.MovLit.Services;
using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Huge.MovLit.Enums;

namespace Huge.Inventory
{
    public partial class Index : ModuleBase, IDisposable
    {
        [Inject] protected ISettingService SettingService { get; set; }
        [Inject] protected InventoryService InventoryService { get; set; }

        private string _settingsUrl;
        private string _filterType = "";
        private List<InventoryItem> _allItems = new();
        private bool disposedValue;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged += PropertyChanged;

                _settingsUrl = EditUrl("Settings", $"returnurl={WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString())}&tab=ModuleSettings");
                
                await LoadSettings();
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Loading Inventory Module {Error}", ex.Message);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!ShouldRender()) return;

            try
            {
                await LoadSettings();
                RefreshInventory();
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error in OnParametersSetAsync {Error}", ex.Message);
            }
        }

        private async Task LoadSettings()
        {
            var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
            _filterType = SettingService.GetSetting(moduleSettings, "InventoryType", "");
        }

        private void RefreshInventory()
        {
            if (!string.IsNullOrEmpty(_filterType))
            {
                _allItems = InventoryService.GetAllByType(_filterType).ToList();
            }
            else
            {
                _allItems = new List<InventoryItem>();
            }
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == NotifyPropertyName.InventoryRefreshed)
            {
                // Refresh when inventory changes via Ink
                RefreshInventory();
                StateHasChanged();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged -= PropertyChanged;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
