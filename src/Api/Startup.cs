using BoardGameWebService.Models;
using DAL;
using DAL.Interfaces;
using Domain.Api.Interfaces;
using Domain.Api.Services;
using Domain.Logger;
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
using Services.Game;
using Services.Lobby;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BoardGameWebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string serviceArg = getServiceArg();
            bool isAllService = serviceArg == null;

            #region Auth
            if (isAllService || serviceArg == "auth")
            {
                // jwt config
                IConfigurationSection jwtC = Configuration.GetSection("JWTTokens");
                JWTConfigModel jwtConfig = new JWTConfigModel();
                jwtC.Bind(jwtConfig);
                services
                    .Configure<JWTConfigModel>((c) => { jwtC.Bind(c); })
                    .AddSingleton((sp) =>
                    {
                        ILogger<JWTEvent> logger = sp.GetService<ILogger<JWTEvent>>();
                        return new JWTEvent(logger);
                    })
                    .AddScoped<IUserInfoDAL, UserInfoDAL>()
                    .AddScoped<IAuthService, AuthService>()
                    .AddSingleton<IJWTService, JWTService>()
                    .AddDbContext<MemberContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("MemberDb"));
                    });

                services
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            }
            #endregion

            #region Game
            if (isAllService || serviceArg == "game")
            {
                services
                    .AddSingleton<ConfigService>()
                    .AddSingleton<IGameService, GameService>()
                    .AddDbContext<GameContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("GameDb"));
                    })
                    .AddScoped<IGameInfoDAL, GameInfoDAL>();
            }
            #endregion

            #region Lobby
            if (isAllService || serviceArg == "lobby")
            {
                services.AddSingleton<IRedisService>(new RedisService(Configuration.GetConnectionString("Redis")));
            }
            #endregion


            services.AddScoped<IResponseService, ResponseService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(initSwagger);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            string serviceArg = getServiceArg();
            loggerFactory.CreateLogger<Startup>()
                .LogInformation("serve {0}", (serviceArg == null) ? "all" : serviceArg);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            app.UseMiddleware<HttpLoggerMiddleware>();

            #region Auth
            app.UseAuthentication();
            #endregion

            app.UseMvc();
        }

        private void initSwagger(SwaggerGenOptions c)
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
                    Description = "在JWT前面加上User Json",
                    Name = "Authorization",
                    Type = "apiKey"
                });
            c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {
                        JwtBearerDefaults.AuthenticationScheme,
                        new string[]{ }
                    },
                });

            string xmlFile = "docs.xml";
            string xmlPath = Path.Combine(_env.ContentRootPath, xmlFile);
            c.IncludeXmlComments(xmlPath);
        }

        private string getServiceArg() => Configuration.GetValue<string>("service");
    }
}
