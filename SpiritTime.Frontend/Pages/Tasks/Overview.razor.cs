using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Services.TableServices;
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TaskModels;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Tasks
{
    public partial class Overview
    {
        [Inject] private ITaskService Service { get; set; }
        [Inject] private IOverlayModalService Modal { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        private bool ShowError { get; set; } = false;
        private string ErrorMsg { get; set; }
        private bool NoElements { get; set; }
        public TaskDto CurrentItem { get; set; }
        public TaskDto SelectedItem { get; set; }
        
        private List<TaskDto> TaskDtoList { get; set; }
        protected override async Task OnInitializedAsync()
        {
            TaskDtoList = new List<TaskDto>();
            var result = await Service.GetAllAsync();
            if (result.Successful)
            {
                if (result.ItemList?.Count > 0)
                {
                    TaskDtoList = result.ItemList;
                    CurrentItem = result.ItemList.FirstOrDefault(x => x.EndDate == DateTime.MinValue);
                }
                else
                {
                    CurrentItem = new TaskDto();
                    NoElements = true;
                }
            }
            else
            {
                ErrorMsg = result.Error;
                ShowError = true;
            }
        }

        private async Task AddAndStartTimer()
        {
            
        }
        void OnRowUpdating(SavedRowItem<TaskDto, Dictionary<string, object>> e) {
            //await TaskDtoList.Update(dataItem, newValue);
            var item = e.Item;
            var name = item.Name;
            Console.WriteLine("Dataitem: " + name);
        }
        private void Update(TaskDto item)
        {
            var parameters = new OverlayModalParameters();
            parameters.Add(SD.Item, item);

            Modal.OnClose += EditResult;
            Modal.Show<Edit>(TextMsg.TaskEdit, parameters);
        }
        private void EditResult(OverlayModalResult modalResult)
        {
            if(!modalResult.Cancelled && modalResult.Data != null)
            {
                var item = (TaskDto)modalResult.Data;
                
                if(item != null)
                {
                    var itemOld = TaskDtoList.FirstOrDefault(x => x.Id == item.Id);
                    TaskDtoList.Remove(itemOld);
                    TaskDtoList.Add(item);
                    StateHasChanged();
                }
            }
            Modal.OnClose -= EditResult;
        }
    }
}