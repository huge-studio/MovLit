using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using System.Collections.Generic;

namespace Huge.Lottie
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "LottiePlayer",
            Description = "Lottie Animations Module",
            Version = "1.0.0",
            ServerManagerType = "Huge.MovLit.Manager.MyModuleManager, Huge.MovLit.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Huge.MovLit.Shared.Oqtane",
            PackageName = "Huge.MovLit",
            Resources = new List<Resource>()
            {
                new Resource { ResourceType = ResourceType.Stylesheet,  Url = "~/Module.css" },
                new Resource { Location = ResourceLocation.Head, Level = ResourceLevel.Site, ResourceType = ResourceType.Script,  Url = "~/Module.js" },
                new Resource {ResourceType = ResourceType.Script, Location = ResourceLocation.Head, Level = ResourceLevel.Site, Url = "https://unpkg.com/@lottiefiles/dotlottie-wc@0.6.2/dist/dotlottie-wc.js", Type= "module"}
            }
        };
    }
}
