using BoardGameWebService.Models;
using Domain.Api.Interfaces;
using Domain.Api.Services;
using Domain.Logger;
using MemberRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Services.Auth;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BoardGameWebService
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
            // jwt config
            IConfigurationSection jwtC = Configuration.GetSection("JWTTokens");
            JWTConfigModel jwtConfig = new JWTConfigModel();
            jwtC.Bind(jwtConfig);

            services.Configure<JWTConfigModel>((c) => { jwtC.Bind(c); });

            services.AddSingleton((sp) =>
            {
                ILogger<JWTEvent> logger = sp.GetService<ILogger<JWTEvent>>();
                return new JWTEvent(logger);
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = jwtConfig.ValidIssuer,
                      ValidAudience = jwtConfig.ValidAudience,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.IssuerSigningKey)),
                      RequireExpirationTime = true,
                  };
                  options.EventsType = typeof(JWTEvent);
              });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<MemberContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("MemberDb"));
            });
            services.AddScoped<IUserInfoDAL, UserInfoDAL>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddSingleton<IJWTService, JWTService>();

            services.AddScoped<IResponseService, ResponseService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    // name: ���� SwaggerDocument �� URL ��m�C
                    name: "development",
                    // info: �O�Ω� SwaggerDocument ������T�����(���e�D����)�C
                    info: new Info
                    {
                        Title = "Auth API",
                        Version = "1.0.0",
                        Description = "�{�һP���v",
                        TermsOfService = "https://github.com/david511382/BoardGame",
                        Contact = new Contact
                        {
                            Name = "David",
                            Url = "https://github.com/david511382/BoardGame"
                        }
                    }
                );

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "�bJWT�e���[�WUser Json",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {
                        JwtBearerDefaults.AuthenticationScheme,
                        new string[]{ }
                    },
                });

                string xmlFile = "Api.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<HttpLoggerMiddleware>();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    // url: �ݰt�X SwaggerDoc �� name�C "/swagger/{SwaggerDoc name}/swagger.json"
                    url: "/swagger/development/swagger.json",
                    // name: �Ω� Swagger UI �k�W����ܤ��P������ SwaggerDocument ��ܦW�٨ϥΡC
                    name: "development"
                );
            });

            app.UseMvc();
        }
    }
}
