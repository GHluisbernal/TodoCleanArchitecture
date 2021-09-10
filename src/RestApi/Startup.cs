using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.RestApi.Modules;
using CleanArchitecture.RestApi.Modules.Common;
using CleanArchitecture.RestApi.Modules.Common.FeatureFlags;
using CleanArchitecture.RestApi.Modules.Common.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace CleanArchitecture.RestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddFeatureFlags(Configuration) // should be the first one.
                .AddInvalidRequestLogging()
                .AddHealthChecks(Configuration)
                .AddAuthentication(Configuration)
                .AddVersioning()
                .AddSwagger()
                .AddApplication()
                .AddInfrastructure(Configuration)
                .AddCustomControllers()
                .AddCustomCors()
                .AddProxy()
                .AddCustomDataProtection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app
                .UseProxy(Configuration)
                .UseHealthChecks("/health")
                .UseHttpsRedirection()
                .UseCustomCors()
                .UseCustomHttpMetrics()
                .UseStaticFiles()
                .UseRouting()
                .UseVersionedSwagger(provider, Configuration, env)
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapMetrics();
                });
        }
    }
}
