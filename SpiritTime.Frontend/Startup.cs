using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using BlazorContextMenu;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpiritTime.Core.Contracts;
using SpiritTime.Frontend.Config;
using SpiritTime.Frontend.ElectronConfig;
using SpiritTime.Frontend.Pages.Tasks;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services;
using SpiritTime.Frontend.Services.AuthServices;
using SpiritTime.Frontend.Services.OptionsService;
using SpiritTime.Frontend.Services.TableServices;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Frontend.Services.TaskTagRuleServices;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Frontend.Shared;

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
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IOptionService, OptionService>();
            services.AddScoped<IOverlayModalService, OverlayModalService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskTagRuleService, TaskTagRuleService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();

            services.AddScoped(typeof(ITableService<>), typeof(TableService<>));


            services.AddSingleton<HttpClient>();
            services.AddSingleton<SelectState>();

            services.AddBlazorContextMenu(options =>
                options.ConfigureTemplate("dark", template =>
                {
                    template.MenuCssClass                = "dark-menu";
                    template.MenuItemCssClass            = "dark-menu-item";
                    template.MenuItemDisabledCssClass    = "dark-menu-item--disabled";
                    template.MenuItemWithSubMenuCssClass = "dark-menu-item--with-submenu";
                    template.Animation                   = Animation.Slide;
                })
            );

            services.AddAuthorization();
            services.AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true; // optional
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
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
            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            // Open the Electron-Window here

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
                // BlazorConfig.CreateTray(env);
            }
            // Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
        }

        private async void ElectronBootstrap()
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Title  = "SpiritTimes",
                Width  = 1152,
                Height = 940,
                Show   = false
            });

            // await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("SpiritTime");
            Electron.Menu.SetApplicationMenu(ElectronMenu.Get());

            Electron.GlobalShortcut.Register(Configuration["Shortcuts:NewTask"], async () =>
            {
                var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/newtask";
                var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(BlazorConfig.GetMiniWindowOptions(), viewPath);
                secondaryWindow.OnClose += browserWindow.Reload;
            });
            Electron.GlobalShortcut.Register(Configuration["Shortcuts:CurrentTask"], async () =>
            {
                var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/newtask/current";
                var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(BlazorConfig.GetMiniWindowOptions(), viewPath);
                secondaryWindow.OnClose += browserWindow.Reload;
            });

            Electron.App.WillQuit += (args) => Task.Run(() => Electron.GlobalShortcut.UnregisterAll());
        }
    }
}