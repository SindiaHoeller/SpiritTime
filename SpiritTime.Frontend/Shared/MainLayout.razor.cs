using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Infrastructure.Config;
using SpiritTime.Frontend.Infrastructure.ElectronConfig;

namespace SpiritTime.Frontend.Shared
{
    public partial class MainLayout
    {
        [Inject] public IOptions<AppSettings> Settings { get; set; }
        public string Version { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (HybridSupport.IsElectronActive)
            {
                var version = await Electron.App.GetVersionAsync();
                Version                      =  "Version: " + version;
                Electron.AutoUpdater.OnError += (message) =>
                {
                    Electron.Notification.Show(new NotificationOptions("Update Check failed!", "Click for more Information.")
                    {
                        OnClick = () => Electron.Dialog.ShowErrorBox("Error", message)
                    });
                };
                Electron.AutoUpdater.OnCheckingForUpdate += async () =>
                {
                    Version = "Checking for Updates...";
                    await InvokeAsync(StateHasChanged);
                };
                Electron.AutoUpdater.OnUpdateNotAvailable += (info) =>
                {
                    Version = "Version: " +  version;
                };
                Electron.AutoUpdater.OnUpdateAvailable += async (info) =>
                {
                    Version = "Update available" + info.Version;
                    await InvokeAsync(StateHasChanged);
                };
                Electron.AutoUpdater.OnDownloadProgress += async (info) =>
                {
                    Version = ElectronHelper.GetDownloadInfo(info);
                    await InvokeAsync(StateHasChanged);
                };
                Electron.AutoUpdater.OnUpdateDownloaded += async (info) =>
                {
                    Version = "Update Complete! Please restart the App to install.";
                    await InvokeAsync(StateHasChanged);
                    Electron.Notification.Show(new NotificationOptions("Update complete! " + info.Version, "Please restart the application!")
                    {
                        OnClick = ElectronConfiguration.CloseApp
                    });
                };
                await ElectronUpdater.Check();
            }
            else
            {
                Version = "Version: " + Settings.Value.Version;
            }
    
        }
        protected void ReloadUi()
        {
            var mainWindow = Electron.WindowManager.BrowserWindows.FirstOrDefault();
            mainWindow?.Reload();
        }

    }
}