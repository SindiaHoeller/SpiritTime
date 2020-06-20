using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using ElectronNET.API;

namespace SpiritTime.Frontend.Infrastructure.ElectronConfig
{
    public static class ElectronUpdater
    {
        public static async Task Init()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.AutoUpdater.OnError              += (message) => Electron.Dialog.ShowErrorBox("Error", message);
                Electron.AutoUpdater.OnCheckingForUpdate  += async () => await Electron.Dialog.ShowMessageBoxAsync("Checking for Updates...");
                Electron.AutoUpdater.OnUpdateNotAvailable += async (info) => await Electron.Dialog.ShowMessageBoxAsync("Update not available");
                Electron.AutoUpdater.OnUpdateAvailable    += async (info) => await Electron.Dialog.ShowMessageBoxAsync("Update available" + info.Version);
                Electron.AutoUpdater.OnDownloadProgress += (info) =>
                {
                    var message1    = "Download speed: " + info.BytesPerSecond + "\n<br/>";
                    var message2    = "Downloaded " + info.Percent + "%"       + "\n<br/>";
                    var message3    = $"({info.Transferred}/{info.Total})"     + "\n<br/>";
                    var message4    = "Progress: " + info.Progress             + "\n<br/>";
                    var information = message1 + message2 + message3           + message4;

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    Electron.IpcMain.Send(mainWindow, "auto-update-reply", information);
                };
                Electron.AutoUpdater.OnUpdateDownloaded += async (info) => await Electron.Dialog.ShowMessageBoxAsync("Update complete!" + info.Version);


                var    currentVersion    = await Electron.App.GetVersionAsync();
                var    updateCheckResult = await Electron.AutoUpdater.CheckForUpdatesAndNotifyAsync();
                var    availableVersion  = updateCheckResult.UpdateInfo.Version;
                await Electron.Dialog.ShowMessageBoxAsync($"Current version: {currentVersion} - available version: {availableVersion}");
            }
        }

        public static async Task Check()
        {
            await Electron.AutoUpdater.CheckForUpdatesAndNotifyAsync();
        }

        public static async Task<string> CheckVersion()
        {
            var    currentVersion    = await Electron.App.GetVersionAsync();
            var    updateCheckResult = await Electron.AutoUpdater.CheckForUpdatesAndNotifyAsync();
            var    availableVersion  = updateCheckResult.UpdateInfo.Version;
            string information       = $"Current version: {currentVersion} - available version: {availableVersion}";

            return information;
            // var mainWindow = Electron.WindowManager.BrowserWindows.First();
            // Electron.IpcMain.Send(mainWindow, "auto-update-reply", information);
        }
    }
}