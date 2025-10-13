using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Themes;
using Oqtane.Shared;

namespace Huge.MovLit.MovLitTheme
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "MovLitTheme",
            Version = "1.0.0",
            PackageName = "Huge.MovLit",
            ThemeSettingsType = "Huge.MovLit.MovLitTheme.ThemeSettings, Huge.MovLit.Client.Oqtane",
            ContainerSettingsType = "Huge.MovLit.MovLitTheme.ContainerSettings, Huge.MovLit.Client.Oqtane",
            Resources = new List<Resource>()
            {
                new Stylesheet(Constants.BootstrapStylesheetUrl, Constants.BootstrapStylesheetIntegrity, "anonymous"),
                new Stylesheet("~/Theme.css"),
                new Script(Constants.BootstrapScriptUrl, Constants.BootstrapScriptIntegrity, "anonymous")
            }
        };
    }
}
