using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;

namespace OcelotApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLogBuilder.ConfigureNLog("NLog.config");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);

            builder.ConfigureServices(s => s.AddSingleton(builder))
                .ConfigureAppConfiguration(
                    (hostContext, config) =>
                    {
                        IHostingEnvironment env = hostContext.HostingEnvironment;
                        config.SetBasePath(Path.Combine(env.ContentRootPath, "."))
                            .AddJsonFile(path: "configuration.json", optional: false, reloadOnChange: true)
                            .AddJsonFile(path: $"configuration.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    })
                .UseNLog()
                .UseStartup<Startup>()
                .ConfigureLogging((hostContext, logging) =>
                {
                    IHostingEnvironment env = hostContext.HostingEnvironment;
                    string configFileName = (env.EnvironmentName.Equals("Release")) ?
                    $"appsettings.json" :
                    $"appsettings.{env.EnvironmentName}.json";
                    IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Path.Combine(env.ContentRootPath, "."))
                        .AddJsonFile(path: configFileName, optional: true, reloadOnChange: true)
                        .Build();
                    logging.ClearProviders();
                    logging.AddConfiguration(configuration.GetSection("Logging"));
                });

            return builder;
        }
    }
}
