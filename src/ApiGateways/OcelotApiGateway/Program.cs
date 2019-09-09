using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace OcelotApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                .UseStartup<Startup>();

            return builder;
        }
    }
}
