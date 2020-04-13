using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        [Inject] private ITableService<TaskDto> TableService { get; set; }
        [Inject] private ITaskService Service { get; set; }
        [Inject] private IOverlayModalService Modal { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        private bool ShowError { get; set; } = false;
        private string ErrorMsg { get; set; }
        private bool NoElements { get; set; }
        public TaskDto CurrentItem { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetAllAsync();
            if (result.Successful)
            {
                if (result.ItemList?.Count > 0)
                {
                    TableService.Objects = result.ItemList;
                    CurrentItem = result.ItemList.FirstOrDefault(x => x.EndDate == DateTime.MinValue);
                }
                else
                {
                    TableService.Objects = new List<TaskDto>();
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
                    var itemOld = TableService.Objects.FirstOrDefault(x => x.Id == item.Id);
                    TableService.Objects.Remove(itemOld);
                    TableService.Objects.Add(item);
                    StateHasChanged();
                }
            }
            Modal.OnClose -= EditResult;
        }
    }
}