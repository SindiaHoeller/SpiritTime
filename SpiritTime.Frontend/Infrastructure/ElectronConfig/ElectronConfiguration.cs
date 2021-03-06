using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SpiritTime.Frontend.Infrastructure.Config;
using WebSocket4Net.Command;

namespace SpiritTime.Frontend.Infrastructure.ElectronConfig
{
    public static class ElectronConfiguration
    {
        public static (IConfiguration, string) SetAppSettings()
        {
            try
            {
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Console.WriteLine("APPSETTINGS: " + homePath);
                if (homePath.ToLower().Contains("appdata"))
                {
                    var appPath = homePath + "\\spirittime";
                    Console.WriteLine("CreateDirectory");
                    Directory.CreateDirectory(appPath);
                    Console.WriteLine("File Exists");
                    if (!File.Exists(appPath + "\\appsettings.json"))
                    {
                        Console.WriteLine("File Copy");
                        File.Copy(Directory.GetCurrentDirectory() + "\\appsettings.json", appPath + "\\appsettings.json");
                    }

                    Console.WriteLine("Build");
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(appPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddEnvironmentVariables();
                    Console.WriteLine("Return");
                    return (builder.Build(), appPath);
                }

                return (null, "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (null, "");
            }
        }


        public static void SetGlobalKeyboardShortcuts(ShortcutsConfig shortcutsConfig, ProxyConfig proxyConfig)
        {
            SetGlobalKeyboardShortcuts(shortcutsConfig, GetMainWindow(), proxyConfig);
        }

        public static void SetGlobalKeyboardShortcuts(
            ShortcutsConfig shortcutsConfig,
            BrowserWindow   mainWindow,
            ProxyConfig     proxyConfig)
        {
            Electron.GlobalShortcut.UnregisterAll();
            Electron.GlobalShortcut.Register(shortcutsConfig.NewTask,         async () => { await CreateNewTaskWindow(mainWindow, proxyConfig); });
            Electron.GlobalShortcut.Register(shortcutsConfig.CurrentTask,     async () => { await CreateCurrentTaskWindow(mainWindow, proxyConfig); });
            Electron.GlobalShortcut.Register(shortcutsConfig.StopCurrentTask, async () => { await CreateStopTasksWindow(mainWindow, proxyConfig); });
        }

        private static async Task CreateNewTaskWindow(BrowserWindow mainWindow, ProxyConfig proxyConfig = null)
        {
            var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/newtask";
            var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(GetMiniWindowOptions(), viewPath);
            // var primaryScreen = await Electron.Screen.GetPrimaryDisplayAsync();
            // secondaryWindow.SetBounds(primaryScreen.WorkArea);
            if (proxyConfig != null)
                await secondaryWindow.WebContents.Session.SetProxyAsync(proxyConfig);
            secondaryWindow.OnClose += mainWindow.Reload;
            SecureHangedProcesses(secondaryWindow);
        }

        private static async Task CreateCurrentTaskWindow(BrowserWindow mainWindow, ProxyConfig proxyConfig = null)
        {
            var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/newtask/current";
            var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(GetMiniWindowOptions(), viewPath);
            if (proxyConfig != null)
                await secondaryWindow.WebContents.Session.SetProxyAsync(proxyConfig);
            secondaryWindow.OnClose += mainWindow.Reload;
            SecureHangedProcesses(secondaryWindow);
        }

        private static async Task CreateStopTasksWindow(BrowserWindow mainWindow, ProxyConfig proxyConfig = null)
        {
            var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/stoptasks";
            var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(GetHiddenWindowOptions(), viewPath);
            if (proxyConfig != null)
                await secondaryWindow.WebContents.Session.SetProxyAsync(proxyConfig);
            secondaryWindow.OnClose += mainWindow.Reload;
            SecureHangedProcesses(secondaryWindow);
        }

        public static async Task<string> SetProxy(ProxyConfig proxyConfig, string serverUrl)
        {
            var window = GetMainWindow();
            if (window != null)
            {
                Console.WriteLine(proxyConfig.ProxyRules);
                await window.WebContents.Session.SetProxyAsync(proxyConfig);
                return await window.WebContents.Session.ResolveProxyAsync(serverUrl);
            }
            else
            {
                return "No window found.";
            }
        }

        public static BrowserWindow GetMainWindow()
        {
            return Electron.WindowManager.BrowserWindows.FirstOrDefault();
        }

        public static BrowserWindowOptions GetMiniWindowOptions()
        {
            return new BrowserWindowOptions
            {
                Width       = 800,
                Height      = 40,
                Center      = true,
                Resizable   = false,
                Maximizable = false,
                AlwaysOnTop = true,
                Frame       = false,
                // Modal = true,
                AcceptFirstMouse = true,
                AutoHideMenuBar  = true
            };
        }

        public static BrowserWindowOptions GetHiddenWindowOptions()
        {
            return new BrowserWindowOptions
            {
                Width       = 0,
                Height      = 0,
                AlwaysOnTop = false,
                Resizable   = false,
                Movable     = false,
                Maximizable = false,
                Frame       = false,
                Closable    = false
            };
        }

        public static async void CreateTray(IWebHostEnvironment env, ShortcutsConfig shortcutsConfig)
        {
            var windowManager = Electron.WindowManager.BrowserWindows.ToList();
            if (Electron.Tray.MenuItems.Count == 0)
            {
                var menu = new[]
                {
                    new MenuItem
                    {
                        Label = "Open SpiritTime",
                        Icon  = Path.Combine(env.ContentRootPath, "Assets/icon24.png"),
                        Click = () =>
                        {
                            Console.WriteLine("Focused on: " + windowManager.FirstOrDefault()?.Id);
                            windowManager.FirstOrDefault()?.Focus();
                        }
                    },
                    new MenuItem
                    {
                        Label       = "Add new Task",
                        Accelerator = shortcutsConfig.NewTask,
                        Icon        = Path.Combine(env.ContentRootPath, "Assets/add.png"),
                        Click = async () =>
                        {
                            Console.WriteLine("Creating new task window.");
                            await CreateNewTaskWindow(windowManager.FirstOrDefault());
                        }
                    },
                    new MenuItem
                    {
                        Label       = "Edit current Task",
                        Accelerator = shortcutsConfig.CurrentTask,
                        Icon        = Path.Combine(env.ContentRootPath, "Assets/edit.png"),
                        Click = async () =>
                        {
                            Console.WriteLine("Creating current task window.");
                            await CreateCurrentTaskWindow(windowManager.FirstOrDefault());
                        }
                    },
                    new MenuItem
                    {
                        Label       = "Stop current Task",
                        Accelerator = shortcutsConfig.StopCurrentTask,
                        Icon        = Path.Combine(env.ContentRootPath, "Assets/stop.png"),
                        Click = async () =>
                        {
                            Console.WriteLine("Stopping current task.");
                            await CreateCurrentTaskWindow(windowManager.FirstOrDefault());
                        }
                    },

                    new MenuItem
                    {
                        Label       = "Reload",
                        Icon        = Path.Combine(env.ContentRootPath, "Assets/renew.png"),
                        Accelerator = "CmdOrCtrl+R",
                        Click = () =>
                        {
                            // on reload, start fresh and close any old
                            // open secondary windows
                            windowManager.ForEach(browserWindow =>
                            {
                                if (browserWindow != windowManager.FirstOrDefault())
                                {
                                    Console.WriteLine("Closing: " + browserWindow.Id);
                                    browserWindow.Close();
                                }
                                else
                                {
                                    Console.WriteLine("Reloading: " + browserWindow.Id);
                                    browserWindow.Reload();
                                }
                            });
                        }
                    },
                    new MenuItem
                    {
                        Label       = "Close",
                        Icon        = Path.Combine(env.ContentRootPath, "Assets/close.png"),
                        Accelerator = "CmdOrCtrl+Q",
                        Click       = CloseApp
                    },
                };

                // var menu = new MenuItem
                // {
                //     Label       = "Reload",
                //     Accelerator = "CmdOrCtrl+R",
                //     Click = () =>
                //     {
                //         // on reload, start fresh and close any old
                //         // open secondary windows
                //         Electron.WindowManager.BrowserWindows.ToList().ForEach(browserWindow =>
                //         {
                //             Console.WriteLine(browserWindow.Id);
                //             if (browserWindow != Electron.WindowManager.BrowserWindows.FirstOrDefault())
                //             {
                //                 browserWindow.Close();
                //             }
                //             else
                //             {
                //                 browserWindow.Reload();
                //             }
                //         });
                //     }
                // };
                var trayIcon = OperatingSystem.IsWindows() ? "Assets/icon64.png" : "Assets/icon32.png";
                Electron.Tray.Show(Path.Combine(env.ContentRootPath, trayIcon), menu);

                Electron.Tray.SetToolTip("SpiritTime");
            }
            else
            {
                Electron.Tray.Destroy();
            }
        }

        public static async void CloseApp()
        {
            Electron.GlobalShortcut.UnregisterAll();
            var windowManager = Electron.WindowManager.BrowserWindows.ToList();
            foreach (var x in windowManager)
            {
                Console.WriteLine("This is a window: " + x.Id);
                if(x != windowManager.FirstOrDefault())
                    x.Close();
            }
            Electron.Tray.Destroy();
        }

        public static MessageBoxOptions GetMessageBoxOptions(string title, string details = "")
        {
            return new MessageBoxOptions(title)
            {
                Title  = title,
                Detail = details,
                Icon   = Path.Combine("/img/icon24.png"),
            };
        }

        public static NotificationOptions GetNotificationOptions(string title, string details, IWebHostEnvironment env)
        {
            return new NotificationOptions(title, details)
            {
                OnClick = () => Electron.WindowManager.BrowserWindows.FirstOrDefault()?.Focus(),
                Icon    = Path.Combine(env.ContentRootPath, "Assets/icon32.png")
            };
        }

        public static void RestartApplication()
        {
            Electron.App.Relaunch();
            Electron.App.Quit();
        }

        public static void SecureHangedProcesses(BrowserWindow browserWindow)
        {
            browserWindow.WebContents.OnCrashed += async (killed) => { MessageForHangCrash(browserWindow, "Renderer Process Crashed", "This process has crashed."); };

            browserWindow.OnUnresponsive += async () => { MessageForHangCrash(browserWindow, "Renderer Process Hanging", "This process is hanging."); };
        }

        private static async void MessageForHangCrash(BrowserWindow browserWindow, string title, string message)
        {
            var options = new MessageBoxOptions(message)
            {
                Type    = MessageBoxType.info,
                Title   = title,
                Buttons = new string[] {"Reload", "Close"}
            };
            var result = await Electron.Dialog.ShowMessageBoxAsync(options);

            if (result.Response == 0)
            {
                browserWindow.Reload();
            }
            else
            {
                browserWindow.Close();
            }
        }
    }
}