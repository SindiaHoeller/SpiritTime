using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Infrastructure.ElectronConfig;
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Pages.QuickInput
{
    public partial class StopTask
    {
        [Inject] private ITaskService Service { get; set; }
        [Inject] private IJSRuntime JsRuntime { get; set; }
        [Inject] private IWebHostEnvironment env { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.StopAllRunningTasks();
            if (HybridSupport.IsElectronActive)
            {
                if (result.Successful)
                {
                    Electron.Notification.Show(ElectronConfiguration.GetNotificationOptions("Success", "Current Task got stopped.", env));
                }else
                {
                    Electron.Notification.Show(ElectronConfiguration.GetNotificationOptions("Error", result.Error, env));
                }
                await JsHelper.CloseWindow(JsRuntime);
            }
        }
    }

}