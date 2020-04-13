using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Services.TableServices;
using SpiritTime.Frontend.Services.TaskTagRuleServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Frontend.Pages.Rules
{
    public partial class Overview
    {
        [Inject] private ITableService<TaskTagRuleDto> TableService { get; set; }
        [Inject] private ITaskTagRuleService Service { get; set; }
        [Inject] private IOverlayModalService Modal { get; set; }
        public bool ShowError { get; set; } = false;
        public string ErrorMsg { get; set; }
        public bool NoElements { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetAllAsync();
            if (result.Successful)
            {
                if (result.ItemList?.Count > 0)
                    TableService.Objects = result.ItemList;
                else
                {
                    TableService.Objects = new List<TaskTagRuleDto>();
                    NoElements = true;
                }
                    
            }
            else
            {
                ErrorMsg = result.Error;
                ShowError = true;
            }
        }
        private void Remove(int id)
        {
            var parameters = new OverlayModalParameters();
            parameters.Add(SD.Id, id);

            Modal.OnClose += RemoveResult;
            Modal.Show<Delete>(TextMsg.TagRuleRemove, parameters);
        }
        private void RemoveResult(OverlayModalResult modalResult)
        {
            if (!modalResult.Cancelled && modalResult.Data != null)
            {
                Int32.TryParse(modalResult.Data.ToString(), out int itemId);
                var item = TableService.Objects.FirstOrDefault(x => x.Id == itemId);
                if (itemId != 0)
                {
                    TableService.Objects.Remove(item);
                    StateHasChanged();
                }
            }

            Modal.OnClose -= RemoveResult;
        }

        private void Add()
        {
            Modal.OnClose += AddResult;
            Modal.Show<Add>(TextMsg.TagRuleAdd);
        }
        private void AddResult(OverlayModalResult modalResult)
        {
            if (!modalResult.Cancelled && modalResult.Data != null)
            {
                var item = (TaskTagRuleDto)modalResult.Data;
                if (item != null)
                {
                    TableService.Objects.Add(item);
                    StateHasChanged();
                }
            }

            Modal.OnClose -= AddResult;
        }

        private void Update(TaskTagRuleDto item)
        {
            var parameters = new OverlayModalParameters();
            parameters.Add(SD.Item, item);

            Modal.OnClose += EditResult;
            Modal.Show<Edit>(TextMsg.TagRuleEdit, parameters);
        }

        private void EditResult(OverlayModalResult modalResult)
        {
            if (!modalResult.Cancelled && modalResult.Data != null)
            {
                var item = (TaskTagRuleDto)modalResult.Data;

                if (item != null)
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