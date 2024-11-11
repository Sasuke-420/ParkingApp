using Lisec.ParkingApp.Utilities;
using Lisec.UserManagementDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lisec.ParkingApp
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        private const string _serviceName = @"ParkingApp";
        private const string _openApiDescription = @"ParkingApp open api service";

        /// <summary>
        /// Constructor of Startup.
        /// </summary>
        /// <param name="env">Specify IWebHostEnvironment.</param>
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddUserManagementDbExtensions();
            services.AddServiceBaseExtensions(_env, false);
            services.AddParkingAppExtensions(_serviceName, _openApiDescription);
            // add DI definitions here
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                if (scope.ServiceProvider.GetService<ParkingAppDbContext>() != null)
                {
                    var dbContextService = scope.ServiceProvider.GetService<ParkingAppDbContext>();
                    dbContextService.Database.Migrate();
                }
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceBaseExtensions(true);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapServiceBaseEndpoints(_serviceName.ToLowerInvariant(), _serviceName);

                var controllerBuilder = endpoints.MapControllers();
                if (!env.IsDevelopment())
                    controllerBuilder.RequireAuthorization();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(@"Hello World!");
                });
            });
        }
    }
}
