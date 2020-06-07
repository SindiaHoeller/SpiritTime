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
    public partial class NewTask
    {
        [Inject] private IJSRuntime JsRuntime { get; set; }
        [Inject] private ITaskService Service { get; set; }
        [Inject] private IWebHostEnvironment env { get; set; }
        [Parameter] public string Current { get; set; }
        private TaskDto TaskDto { get; set; }
        private ElementReference inputBox;
        private ElementReference newTask;

        protected override async Task OnInitializedAsync()
        {
            TaskDto = new TaskDto();
            if (!string.IsNullOrEmpty(Current))
            {
                var result = await Service.GetCurrentTask();
                if (result.Successful)
                {
                    if (result.Item != null)
                    {
                        TaskDto = result.Item;
                    }
                }
                else
                {
                    Electron.Notification.Show(ElectronConfiguration.GetNotificationOptions("Error", result.Error, env));
                }
            }
            else
            {
                TaskDto.Name = await Electron.Clipboard.ReadTextAsync();
            }
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
                    var result = TaskDto.Id == 0 ? await Service.Add(TaskDto) : await Service.Edit(TaskDto);
                    NotificationOptions options;
                    if (result.Successful)
                    {
                        options = TaskDto.Id == 0 
                            ? ElectronConfiguration.GetNotificationOptions(SD.TaskGotCreated, TaskDto.Name + " - " + TaskDto.Description, env) 
                            : ElectronConfiguration.GetNotificationOptions(SD.TaskGotEdited, TaskDto.Name + " - " + TaskDto.Description, env);
                    }
                    else
                    {
                        options = ElectronConfiguration.GetNotificationOptions("Error", result.Error, env);
                    }
                    Electron.Notification.Show(options);
                    await JsHelper.CloseWindow(JsRuntime);
                    
                    // Console.WriteLine($"NewTask submitted...Pressed: [{e.Key}]");
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