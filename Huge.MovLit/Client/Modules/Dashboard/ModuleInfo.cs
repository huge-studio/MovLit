using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using System.Collections.Generic;

namespace Huge.Dashboard
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Dashboard",
            Description = "Stories Dashboard",
            Version = "1.0.0",
            ServerManagerType = "Huge.MovLit.Manager.MyModuleManager, Huge.MovLit.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Huge.MovLit.Shared.Oqtane",
            SettingsType = "Huge.Dashboard.Settings, Huge.MovLit.Client.Oqtane",
            PackageName = "Huge.MovLit",
            Resources = new List<Resource>()
            {
                new Resource { ResourceType = ResourceType.Stylesheet,  Url = "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" },
                new Resource { ResourceType = ResourceType.Stylesheet,  Url = "~/Module.css" }
            }
        };
    }
}
