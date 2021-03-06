﻿using Domain.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotApiGateway
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string allowedHostsStr = Configuration.GetSection("CorsHosts").Value;
            string[] allowedHosts = allowedHostsStr.Split(',');
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(allowedHosts)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            string identityUrl = Configuration.GetValue<string>("IdentityUrl");
            services.AddSingleton<Auth>((sp) =>
            {
                ILogger<Auth> logger = sp.GetService<ILogger<Auth>>();
                return new Auth(identityUrl, logger);
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.EventsType = typeof(Auth);
                });

            services.AddOcelot(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseMiddleware<HttpLoggerMiddleware>();

            app.UseAuthentication();

            app.UseOcelot().Wait();
        }
    }
}
