using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;

namespace LobbyWebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLogBuilder.ConfigureNLog("NLog.config");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
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
    }
}
