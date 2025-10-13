using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Oqtane.Services;
using Huge.MovLit.Services;

namespace Huge.MovLit.Startup
{
    public class ClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(IMyModuleService)))
            {
                services.AddScoped<IMyModuleService, MyModuleService>();
            }

            if (!services.Any(s => s.ServiceType == typeof(StoryService)))
            {
                services.AddTransient<StoryService>();
            }
        }
    }
}
