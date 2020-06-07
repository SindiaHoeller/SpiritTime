using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using SpiritTime.Frontend.Infrastructure.Config;

namespace SpiritTime.Frontend.Infrastructure.ElectronConfig
{
    public static class ElectronConfiguration
    {
        public static void SetGlobalKeyboardShortcuts(ShortcutsConfig shortcutsConfig, ProxyConfig proxyConfig)
        {
            SetGlobalKeyboardShortcuts(shortcutsConfig, GetMainWindow(), proxyConfig);
        }
        public static void SetGlobalKeyboardShortcuts(
            ShortcutsConfig shortcutsConfig,
            BrowserWindow mainWindow,
            ProxyConfig proxyConfig)
        {
            Electron.GlobalShortcut.UnregisterAll();
            Electron.GlobalShortcut.Register(shortcutsConfig.NewTask, async () =>
            {
                await CreateNewTaskWindow(mainWindow, proxyConfig);
            });
            Electron.GlobalShortcut.Register(shortcutsConfig.CurrentTask, async () =>
            {
                await CreateCurrentTaskWindow(mainWindow, proxyConfig);
            });
            Electron.GlobalShortcut.Register(shortcutsConfig.StopCurrentTask, async () =>
            {
                await CreateStopTasksWindow(mainWindow, proxyConfig);
            });
        }

        private static async Task CreateNewTaskWindow(BrowserWindow mainWindow, ProxyConfig proxyConfig = null)
        {
            var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/newtask";
            var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(GetMiniWindowOptions(), viewPath);
            if(proxyConfig != null)
                await secondaryWindow.WebContents.Session.SetProxyAsync(proxyConfig);
            secondaryWindow.OnClose += mainWindow.Reload;
        }

        private static async Task CreateCurrentTaskWindow(BrowserWindow mainWindow, ProxyConfig proxyConfig = null)
        {
            var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/newtask/current";
            var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(GetMiniWindowOptions(), viewPath);
            if(proxyConfig != null)
                await secondaryWindow.WebContents.Session.SetProxyAsync(proxyConfig);
            secondaryWindow.OnClose += mainWindow.Reload;
        }
        private static async Task CreateStopTasksWindow(BrowserWindow mainWindow, ProxyConfig proxyConfig = null)
        {
            var viewPath        = $"http://localhost:{BridgeSettings.WebPort}/stoptasks";
            var secondaryWindow = await Electron.WindowManager.CreateWindowAsync(GetHiddenWindowOptions(), viewPath);
            if(proxyConfig != null)
                await secondaryWindow.WebContents.Session.SetProxyAsync(proxyConfig);
            secondaryWindow.OnClose += mainWindow.Reload;
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
                Movable     = false,
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
                Closable = false
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
                        Label = "Open TickTick",
                        Icon = Path.Combine(env.ContentRootPath, "Assets/icon24.png"),
                        Click = () =>
                        {
                            Console.WriteLine("Focused on: " + windowManager.FirstOrDefault()?.Id);
                            windowManager.FirstOrDefault()?.Focus();
                        }
                    },
                    new MenuItem
                    {
                        Label = "Add new Task",
                        Accelerator = shortcutsConfig.NewTask,
                        Icon = Path.Combine(env.ContentRootPath, "Assets/add.png"),
                        Click = async ()  =>
                        {
                            Console.WriteLine("Creating new task window.");
                            await CreateNewTaskWindow(windowManager.FirstOrDefault());
                        }
                    },
                    new MenuItem
                    {
                        Label = "Edit current Task",
                        Accelerator = shortcutsConfig.CurrentTask,
                        Icon = Path.Combine(env.ContentRootPath, "Assets/edit.png"),
                        Click = async () =>
                        {
                            Console.WriteLine("Creating current task window.");
                            await CreateCurrentTaskWindow(windowManager.FirstOrDefault());
                        }
                    },
                    new MenuItem
                    {
                        Label = "Stop current Task",
                        Accelerator = shortcutsConfig.StopCurrentTask,
                        Icon = Path.Combine(env.ContentRootPath, "Assets/stop.png"),
                        Click = async () =>
                        {
                            Console.WriteLine("Stopping current task.");
                            await CreateCurrentTaskWindow(windowManager.FirstOrDefault());
                        }
                    },

                    new MenuItem
                    {
                        Label       = "Reload",
                        Icon = Path.Combine(env.ContentRootPath, "Assets/renew.png"),
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
                        Label = "Close",
                        Icon = Path.Combine(env.ContentRootPath, "Assets/close.png"),
                        Accelerator = "CmdOrCtrl+Q",
                        Click = () =>
                        {
                            Console.WriteLine("Remove on: " + windowManager.FirstOrDefault()?.Id);
                            Electron.Tray.Destroy();
                            windowManager.ForEach(x=>x.Close());
                        }
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
                
                Electron.Tray.Show(Path.Combine(env.ContentRootPath, "Assets/icon_120x120.png"), menu);
                Electron.Tray.SetToolTip("SpiritTime");
            }
            else
            {
                Electron.Tray.Destroy();
            }
        }

        public static MessageBoxOptions GetMessageBoxOptions(string title, string details = "")
        {
            return new MessageBoxOptions(title)
            {
                Title = title,
                Detail = details,
                Icon = Path.Combine("/img/icon24.png"),
            };
        }

        public static NotificationOptions GetNotificationOptions(string title, string details, IWebHostEnvironment env)
        {
            return new NotificationOptions(title, details)
            {
                OnClick = () => Electron.WindowManager.BrowserWindows.FirstOrDefault()?.Focus(),
                Icon = Path.Combine(env.ContentRootPath, "Assets/icon32.png")
            };
        }
    }
}