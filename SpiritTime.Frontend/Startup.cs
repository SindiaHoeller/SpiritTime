using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services;
using SpiritTime.Frontend.Services.AuthServices;
using SpiritTime.Frontend.Services.OverlayModalService;
using SpiritTime.Frontend.Services.TableServices;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.WorkspaceServices;

namespace SpiritTime.Frontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddAutoMapper(typeof(Startup));

            var appSettingSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);

            services.AddTransient<ValidateHeaderHandler>();

            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            services.AddBlazoredLocalStorage();
            services.AddHttpClient<IAuthService, AuthService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<ITagService, TagService>();

            services.AddScoped<IOverlayModalService, OverlayModalService>();
            services.AddScoped(typeof(ITableService<>), typeof(TableService<>));

            services.AddSingleton<HttpClient>();

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
