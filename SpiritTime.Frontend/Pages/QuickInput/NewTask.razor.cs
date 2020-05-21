using System;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Pages.QuickInput
{
    public partial class NewTask
    {
        [Inject] private IJSRuntime JsRuntime { get; set; }
        [Inject] private ITaskService Service { get; set; }
        private TaskDto TaskDto { get; set; }
        private ElementReference inputBox;
        private ElementReference newTask;

        protected override void OnInitialized()
        {
            TaskDto = new TaskDto();
            
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await inputBox.Focus(JsRuntime);
            }
        }

        private async Task Submit(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Enter":
                {
                    await newTask.Focus(JsRuntime);
                    var result = await Service.Add(TaskDto);
                    if (result.Successful)
                    {
                        var options = new NotificationOptions(SD.TaskGotCreated, TaskDto.Name + " - " + TaskDto.Description)
                        {
                            OnClick = async () => await Electron.Dialog.ShowMessageBoxAsync("Notification clicked")
                        };

                        Electron.Notification.Show(options);
                    }
                    else
                    {
                        var options = new NotificationOptions("Error", result.Error)
                        {
                            OnClick = async () => await Electron.Dialog.ShowMessageBoxAsync("Notification clicked")
                        };

                        Electron.Notification.Show(options);
                    }
                    await JsHelper.CloseWindow(JsRuntime);
                    Console.WriteLine($"NewTask submitted...Pressed: [{e.Key}]");
                    break;
                }
                case "Escape":
                {
                    if (HybridSupport.IsElectronActive)
                    {
                        await JsHelper.CloseWindow(JsRuntime);
                    }
                    break;
                }
            }

            
        }
        // private void KeyDown(KeyboardEventArgs e)
        // {
        //     Console.WriteLine( $"Pressed: [{e.Key}]");
        // }
    }
    
}