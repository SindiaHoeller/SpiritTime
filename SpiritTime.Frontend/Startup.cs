using System;
using System.Collections.Generic;
using System.IO;
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
using SpiritTime.Frontend.Infrastructure.Config;
using SpiritTime.Frontend.Infrastructure.Config.WriteOptions;
using SpiritTime.Frontend.Infrastructure.ElectronConfig;
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
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = "appsettings.json";
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddAutoMapper(typeof(Startup));
            
            services.AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true; // optional
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
            if (HybridSupport.IsElectronActive)
            {
                var (conf, path) = ElectronConfiguration.SetAppSettings();
                Console.WriteLine("AppSettings done: " + appSettings);
                if (conf != null)
                    Configuration = conf;
                appSettings = path + "\\" + appSettings;
                Console.WriteLine("APPSETTINGS2: " + appSettings);
            }
            services.ConfigureWritable<ShortcutsConfig>(Configuration.GetSection("Shortcuts"), appSettings);
            services.ConfigureWritable<ElectronProxyConfig>(Configuration.GetSection("Proxy"), appSettings);
            services.ConfigureWritable<ProxyAuth>(Configuration.GetSection("ProxyAuth"), appSettings);
            services.Configure<AppSettings>(Configuration.GetSection("Settings"));

            services.AddTransient<ValidateHeaderHandler>();

            var proxyEnabled = Convert.ToBoolean(Configuration["ProxyAuth:Enabled"]);
            if (proxyEnabled)
            {
                services.AddHttpClient<IAuthService, AuthService>()
                    .ConfigurePrimaryHttpMessageHandler(() => { return ProxySettings.GetClientHandler(Configuration); });
            
                var client = new HttpClient(ProxySettings.GetClientHandler(Configuration));
                services.AddSingleton(client);
            }
            else
            {
                services.AddHttpClient<IAuthService, AuthService>();
                services.AddSingleton<HttpClient>();
            }
            
            
            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            services.AddBlazoredLocalStorage();
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IOptionService, OptionService>();
            services.AddScoped<IOverlayModalService, OverlayModalService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskTagRuleService, TaskTagRuleService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();

            services.AddScoped(typeof(ITableService<>), typeof(TableService<>));


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

            //app.UseHttpsRedirection();
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
                ElectronBootstrap(env);
            }
        }

        private async void ElectronBootstrap(IWebHostEnvironment env)
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Title  = "SpiritTimes",
                Icon = Path.Combine(env.ContentRootPath, "Assets/icon_120x120.png"),
                Width  = 1152,
                Height = 940,
                Show   = false
            });
            var shortcutConfig = new ShortcutsConfig
            {
                CurrentTask = Configuration["Shortcuts:CurrentTask"],
                NewTask = Configuration["Shortcuts:NewTask"],
                StopCurrentTask = Configuration["Shortcuts:StopCurrentTask"]
            };
            
            // await browserWindow.WebContents.Session.ClearCacheAsync();
            var proxyConfig = new ProxyConfig(Configuration["Proxy:PacScript"], 
                Configuration["Proxy:PacScript"], 
                Configuration["Proxy:ProxyBypassRules"]);
            await browserWindow.WebContents.Session.SetProxyAsync(proxyConfig);
            ElectronConfiguration.SetGlobalKeyboardShortcuts(shortcutConfig, browserWindow, proxyConfig);

            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("SpiritTime");
            Electron.Menu.SetApplicationMenu(ElectronMenu.Get());
            ElectronConfiguration.CreateTray(env, shortcutConfig);
            Electron.App.WillQuit += (args) => Task.Run(() =>
            {
                ElectronConfiguration.CloseApp();
            });
            ElectronConfiguration.SecureHangedProcesses(browserWindow);
        }
    }
}