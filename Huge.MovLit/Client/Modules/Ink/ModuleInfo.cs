using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using System.Collections.Generic;

namespace Huge.Ink
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "InkPlayer",
            Description = "Ink player",
            Version = "1.0.0",
            ServerManagerType = "Huge.MovLit.Manager.InkManager, Huge.MovLit.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Huge.MovLit.Shared.Oqtane,Markdig,ink_compiler,ink-engine-runtime",
            PackageName = "Huge.MovLit",
            Resources = new List<Resource>()
            {
                new Resource { ResourceType = ResourceType.Stylesheet,  Url = "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" },
                // CodeMirror 5 core CSS/JS and enhancements
                new Resource { ResourceType = ResourceType.Stylesheet,  Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/codemirror.min.css" },
                // Optional theme for nicer look
                new Resource { ResourceType = ResourceType.Stylesheet,  Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/theme/neo.min.css" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/codemirror.min.js" },
                // Modes / addons we use
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/mode/markdown/markdown.min.js" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/addon/mode/simple.min.js" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/addon/edit/matchbrackets.min.js" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/addon/edit/closebrackets.min.js" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/addon/selection/active-line.min.js" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/addon/search/search.min.js" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/addon/search/searchcursor.min.js" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/keymap/sublime.min.js" },
                // add this next to your other CodeMirror resources
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script, Url = "https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/addon/mode/simple.min.js" },
                // Module interop to wire CodeMirror
                new Resource { ResourceType = ResourceType.Script, Location = ResourceLocation.Body, Level = ResourceLevel.Site, Url = "~/Module.js" }
            }
        };
    }
}
