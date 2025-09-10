using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Huge.MovLit.Repository;
using Huge.MovLit.Services;

namespace Huge.MovLit.Startup
{
    public class ServerStartup : IServerStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // not implemented
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            // not implemented
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMyModuleService, ServerMyModuleService>();
            services.AddDbContextFactory<Context>(opt => { }, ServiceLifetime.Transient);
        }
    }
}
