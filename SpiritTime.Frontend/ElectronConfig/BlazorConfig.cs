using System;
using System.IO;
using System.Linq;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;

namespace SpiritTime.Frontend.ElectronConfig
{
    public static class BlazorConfig
    {
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

        public static void CreateTray(IWebHostEnvironment env)
        {
            var windowManager = Electron.WindowManager.BrowserWindows.ToList();
            if (Electron.Tray.MenuItems.Count == 0)
            {

                // var menu = new[]
                // {
                //     new MenuItem
                //     {
                //         Label = "Open Main Window",
                //         Click = () =>
                //         {
                //             Console.WriteLine("Focussed on: " + windowManager.FirstOrDefault()?.Id);
                //             windowManager.FirstOrDefault()?.Focus();
                //         }
                //     },
                //     new MenuItem
                //     {
                //         Label = "Reload",
                //         Click = () =>
                //         {
                //             Console.WriteLine("Reload on: " + windowManager.FirstOrDefault()?.Id);
                //             // on reload, start fresh and close any old
                //             // open secondary windows
                //             windowManager.ForEach(browserWindow =>
                //             {
                //                 if (browserWindow.Id != 1)
                //                 {
                //                     browserWindow.Close();
                //                 }
                //                 else
                //                 {
                //                     browserWindow.Reload();
                //                 }
                //             });
                //         }
                //     },
                //     new MenuItem
                //     {
                //         Label = "Remove",
                //         Click = () =>
                //         {
                //             Console.WriteLine("Remove on: " + windowManager.FirstOrDefault()?.Id);
                //             Electron.Tray.Destroy();
                //             windowManager.ForEach(x=>x.Close());
                //         }
                //     },
                // };

                var menu = new MenuItem
                {
                    Label       = "Reload",
                    Accelerator = "CmdOrCtrl+R",
                    Click = () =>
                    {
                        // on reload, start fresh and close any old
                        // open secondary windows
                        Electron.WindowManager.BrowserWindows.ToList().ForEach(browserWindow =>
                        {
                            if (browserWindow.Id != 1)
                            {
                                browserWindow.Close();
                            }
                            else
                            {
                                browserWindow.Reload();
                            }
                        });
                    }
                };
                
                Electron.Tray.Show(Path.Combine(env.ContentRootPath, "Assets/icon32.png"), menu);
                // Electron.Tray.SetToolTip("SpiritTime");
            }
            else
            {
                Electron.Tray.Destroy();
            }
        }
    }
}