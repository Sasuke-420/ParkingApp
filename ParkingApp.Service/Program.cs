using Lisec.Base.Utilities;
using Lisec.ServiceBase.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Lisec.ParkingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    BootstrapConfig.SetUpBootstrapConfig();
                    webBuilder.UseKestrel(configureOptions =>
                    {
                        configureOptions.ListenAnyIP(ServiceBaseUtility.GetPortFromAspNetCoreUrl(), l => l.Protocols = HttpProtocols.Http1AndHttp2);
                        configureOptions.ListenAnyIP(ServiceBaseUtility.GetPortForGRpcServer(), l => l.Protocols = HttpProtocols.Http2);
                    });
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                });
    }
}
