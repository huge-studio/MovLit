using Oqtane.Models;
using Oqtane.Modules;

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
            PackageName = "Huge.MovLit"
        };
    }
}
