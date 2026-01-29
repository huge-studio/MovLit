using Microsoft.AspNetCore.Components;
using Oqtane.Modules;
using Oqtane.Services;
using System.Net;
using System.Threading.Tasks;

namespace Huge.Uploader
{
    public partial class Index : ModuleBase
    {

        private string _returnUrl;
        private string _settingsUrl;

        protected override void OnParametersSet()
        {
            _returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
            _settingsUrl = EditUrl("Settings", $"returnurl={_returnUrl}&tab=ModuleSettings");
        }
    }
}
