using AuthWebService.Models;
using AuthWebService.Sevices;
using Domain.ApiResponse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebService
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
                  options.Events = new JwtBearerEvents()
                  {
                      OnAuthenticationFailed = context =>
                      {
                          context.NoResult();

                          context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                          context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = context.Exception.Message;
                          //Debug.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                          return Task.CompletedTask;
                      },
                      OnTokenValidated = async context =>
                      {
                          Console.WriteLine("OnTokenValidated: " +
                                context.SecurityToken);
                      }
                  };
              });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            string memberDbConnStr = Configuration.GetSection("MemberDbConfig").Value;
            services.AddSingleton<IAuthService>(new AuthService(memberDbConnStr));

            services.AddSingleton<IJWTService, JWTService>();

            services.AddScoped<IResponseService, ResponseService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    // name: 攸關 SwaggerDocument 的 URL 位置。
                    name: "development",
                    // info: 是用於 SwaggerDocument 版本資訊的顯示(內容非必填)。
                    info: new Info
                    {
                        Title = "Auth API",
                        Version = "1.0.0",
                        Description = "認證與授權",
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
                        Description = "在JWT前面加上Bearer與空格",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {
                        JwtBearerDefaults.AuthenticationScheme,
                        new string[]{ }
                    },
                });

                string filePath = Path.Combine(@"./bin/Debug/netcoreapp2.2", "Api.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseIdentityServer();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
                    url: "/swagger/development/swagger.json",
                    // name: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                    name: "development"
                );
            });

            app.UseMvc();
        }
    }
}
