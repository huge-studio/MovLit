using Oqtane.Models;
using Oqtane.Modules;

namespace Huge.MovLit.MyModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "MyModule",
            Description = "Example module",
            Version = "1.0.0",
            ServerManagerType = "Huge.MovLit.Manager.MyModuleManager, Huge.MovLit.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Huge.MovLit.Shared.Oqtane",
            PackageName = "Huge.MovLit" 
        };
    }
}
