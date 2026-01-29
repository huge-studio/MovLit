using Oqtane.Models;
using Oqtane.Modules;

namespace Huge.Uploader
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Uploader",
            Description = "Upload Images and Lottie Files",
            Version = "1.0.0",
            ServerManagerType = "Huge.Uploader.Manager.UploaderManager, Huge.Uploader.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Huge.MovLit.Shared.Oqtane",
            PackageName = "Huge.MovLit" 
        };
    }
}
