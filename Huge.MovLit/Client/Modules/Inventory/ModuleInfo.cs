using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using System.Collections.Generic;

namespace Huge.Inventory
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Inventory",
            Description = "Display collected inventory items",
            Version = "1.0.0",
            ServerManagerType = "Huge.MovLit.Manager.MyModuleManager, Huge.MovLit.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Huge.MovLit.Shared.Oqtane",
            PackageName = "Huge.MovLit",
            Resources = new List<Resource>()
            {
                new Resource { ResourceType = ResourceType.Stylesheet, Url = "~/Module.css" }
            }
        };
    }
}
