using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Huge.Dashboard
{
    public class SettingsViewModel
    {
        private const string SectionsKey = nameof(Sections);
        private const string ShowHeroKey = nameof(ShowHero);
        private const string QuickLinksKey = nameof(QuickLinks);
        private const string PopularTagsKey = nameof(PopularTags);
        private const string FeaturedStoryIdKey = nameof(FeaturedStoryId);
        private const string BlogTakeKey = nameof(BlogTake);

        public List<SectionConfig> Sections { get; set; } = new();
        public bool ShowHero { get; set; } = true;
        public List<QuickLink> QuickLinks { get; set; } = new();
        public List<string> PopularTags { get; set; } = new();
        public int? FeaturedStoryId { get; set; }
        public int BlogTake { get; set; } = 6;

        public SettingsViewModel() { }

        public SettingsViewModel(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            try
            {
                var json = settingService.GetSetting(moduleSettings, SectionsKey, "[]");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var sections = JsonSerializer.Deserialize<List<SectionConfig>>(json, JsonOptions());
                    if (sections != null) Sections = sections;
                }
            }
            catch { Sections = new(); }

            try
            {
                var showHeroStr = settingService.GetSetting(moduleSettings, ShowHeroKey, ShowHero.ToString());
                if (bool.TryParse(showHeroStr, out var sh)) ShowHero = sh;
            }
            catch
            {
                // keep default
            }

            try
            {
                var linksJson = settingService.GetSetting(moduleSettings, QuickLinksKey, "[]");
                if (!string.IsNullOrWhiteSpace(linksJson))
                {
                    var links = JsonSerializer.Deserialize<List<QuickLink>>(linksJson, JsonOptions());
                    if (links != null) QuickLinks = links;
                }
            }
            catch { QuickLinks = new(); }

            try
            {
                var tagsJson = settingService.GetSetting(moduleSettings, PopularTagsKey, "[]");
                if (!string.IsNullOrWhiteSpace(tagsJson))
                {
                    var tags = JsonSerializer.Deserialize<List<string>>(tagsJson, JsonOptions());
                    if (tags != null) PopularTags = tags;
                }
            }
            catch { PopularTags = new(); }

            try
            {
                var featStr = settingService.GetSetting(moduleSettings, FeaturedStoryIdKey, "");
                if (int.TryParse(featStr, out var fid)) FeaturedStoryId = fid;
            }
            catch { FeaturedStoryId = null; }

            try
            {
                var blogTakeStr = settingService.GetSetting(moduleSettings, BlogTakeKey, BlogTake.ToString());
                if (int.TryParse(blogTakeStr, out var bt)) BlogTake = bt;
            }
            catch { BlogTake = 6; }
        }

        public void SetSettings(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            var sectionsJson = JsonSerializer.Serialize(Sections, JsonOptions());
            settingService.SetSetting(moduleSettings, SectionsKey, sectionsJson);
            settingService.SetSetting(moduleSettings, ShowHeroKey, ShowHero.ToString());

            var linksJson = JsonSerializer.Serialize(QuickLinks, JsonOptions());
            settingService.SetSetting(moduleSettings, QuickLinksKey, linksJson);

            var tagsJson = JsonSerializer.Serialize(PopularTags, JsonOptions());
            settingService.SetSetting(moduleSettings, PopularTagsKey, tagsJson);

            settingService.SetSetting(moduleSettings, FeaturedStoryIdKey, FeaturedStoryId?.ToString() ?? "");
            settingService.SetSetting(moduleSettings, BlogTakeKey, BlogTake.ToString());
        }

        private static JsonSerializerOptions JsonOptions() => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            AllowTrailingCommas = true
        };
    }

    public class SectionConfig
    {
        public string Title { get; set; } = "Section";
        public string? ActionText { get; set; } = "View More";
        public int Cards { get; set; } = 9;
        public bool Horizontal { get; set; } = false;
    }

    public class QuickLink
    {
        public string Name { get; set; } = string.Empty; 
        public string Url { get; set; } = string.Empty;  
        public bool IsExternal { get; set; } = false;
        public int? PageId { get; set; } 
    }
}
