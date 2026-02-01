using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Oqtane.Services;
using Oqtane.Shared;

namespace Huge.MovLit.Services
{
    public class InventoryItem
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsCollected { get; set; }
    }

    public class InventoryService : ServiceBase
    {
        private Dictionary<string, InventoryItem> _allItems = new();
        private HashSet<string> _collectedItems = new();
        private readonly SiteState _siteState;

        public InventoryService(HttpClient http, SiteState siteState) : base(http, siteState) 
        {
            _siteState = siteState;
        }

        public void RegisterItem(string name, string type, string icon)
        {
            _allItems[name] = new InventoryItem
            {
                Type = type,
                Name = name,
                Icon = icon
            };
            NotifyInventoryChanged();
        }

        public void RegisterItemJson(string json)
        {
            try
            {
                var item = JsonSerializer.Deserialize<InventoryItem>(json);
                if (item != null && !string.IsNullOrEmpty(item.Name))
                {
                    _allItems[item.Name] = item;
                    NotifyInventoryChanged();
                }
            }
            catch
            {
                // Invalid JSON - ignore
            }
        }

        public void CollectItem(string name)
        {
            if (_allItems.ContainsKey(name))
            {
                _collectedItems.Add(name);
                _allItems[name].IsCollected = true;
                NotifyInventoryChanged();
            }
        }

        public bool HasItem(string name)
        {
            return _collectedItems.Contains(name);
        }

        public int CountByType(string type)
        {
            return _collectedItems
                .Where(name => _allItems.ContainsKey(name) && _allItems[name].Type == type)
                .Count();
        }

        public int TotalByType(string type)
        {
            return _allItems.Values
                .Where(item => item.Type == type)
                .Count();
        }

        public string GetItemIcon(string name)
        {
            return _allItems.TryGetValue(name, out var item) ? item.Icon : "";
        }

        public IEnumerable<InventoryItem> GetCollectedItems()
        {
            return _collectedItems
                .Where(name => _allItems.ContainsKey(name))
                .Select(name => _allItems[name]);
        }

        public IEnumerable<InventoryItem> GetCollectedByType(string type)
        {
            return GetCollectedItems().Where(item => item.Type == type);
        }

        public IEnumerable<InventoryItem> GetAllByType(string type)
        {
            return _allItems.Values.Where(item => item.Type == type);
        }

        public InventoryItem GetItem(string name)
        {
            return _allItems.TryGetValue(name, out var item) ? item : null;
        }

        public void Clear()
        {
            _allItems.Clear();
            _collectedItems.Clear();
            NotifyInventoryChanged();
        }

        private void NotifyInventoryChanged()
        {
            // Notify any listening components that inventory has changed
            _siteState.Properties.InventoryRefreshed = System.DateTime.UtcNow.Ticks.ToString();
        }
    }
}
