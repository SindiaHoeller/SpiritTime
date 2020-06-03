using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpiritTime.Backend.Infrastructure.Jwt;
using SpiritTime.Backend.Services;
using SpiritTime.Core;
using SpiritTime.Core.Entities;
using SpiritTime.Persistence;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;

namespace SpiritTime.Backend
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddOptions();
            services.AddDbContext<ApplicationDbContext>();

            services.Configure<Settings>(Configuration.GetSection("Settings"));
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<JwtAuthentication, JwtAuthentication>();
            services.AddAutoMapper(typeof(Startup));

            //services
            //    .AddMvc()
            //    .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services
                .AddAuthentication(GetAuthenticationOptions)
                .AddJwtBearer(GetJwtBearerOptions);

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // Default Lockout settings.
                    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers      = true;
                    options.SignIn.RequireConfirmedEmail    = true;
                    options.User.RequireUniqueEmail         = true;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title   = "VocaDrill",
                    Version = "v1"
                });
                // c.IncludeXmlComments(GetXmlCommentsPath());
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            var swaggerOptions = new Services.SwaggerOptions();


            Configuration.GetSection(nameof(swaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });
            app.UseSwaggerUI(c => { c.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description); });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("<div style=\"text-align: center;\">"                                                                    +
                                                      "<h1 style=\"text-align: center; margin-top: 20px\">Landing Page -  SpiritTime API</h1>"                 +
                                                      "<a style=\"text-align: center; border: 1px solid black; background-color: black; "                      +
                                                      "border-radius: 5px; color: white; padding: 5px 15px; text-decoration: none; cursor: pointer;\" href=\"" +
                                                      swaggerOptions.LandingPageRedirectUrl                                                                    +
                                                      "\">Go to Swagger</a>"                                                                                   +
                                                      "</div>");
                });
            });
            //app.UseSwagger();
            //app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpiritTime"); });
        }

        private void GetAuthenticationOptions(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        }

        private void GetJwtBearerOptions(JwtBearerOptions options)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer      = Configuration["Jwt:ValidIssuer"],
                ValidAudience    = Configuration["Jwt:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecurityKey"]))
            };
        }

        // private string GetXmlCommentsPath()
        // {
        //     return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SpiritTime.Backend.xml");
        // }
    }
}