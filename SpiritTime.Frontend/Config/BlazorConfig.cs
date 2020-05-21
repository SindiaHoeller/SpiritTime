using ElectronNET.API.Entities;

namespace SpiritTime.Frontend.Config
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
    }
}