using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Core.Entities;
using SpiritTime.Frontend.Services.OverlayModalService;
using SpiritTime.Frontend.Services.TableServices;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.WorkspacePages
{
    public partial class Overview
    {
        [Inject] private ITableService<WorkspaceDto> TableService { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }
        [Inject] private IOverlayModalService Modal { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        public bool ShowError { get; set; } = false;
        public string ErrorMsg { get; set; }
        public bool NoElements { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetAllAsync();
            if (result.Successful)
            {
                if (result.Workspaces?.Count > 0)
                    TableService.Objects = result.Workspaces;
                else
                {
                    TableService.Objects = new List<WorkspaceDto>();
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
            Modal.Show<Delete>(Text.WorkspaceRemove, parameters);
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
            Modal.Show<Add>(Text.AddWorkspace);
        }
        private void AddResult(OverlayModalResult modalResult)
        {
            if (!modalResult.Cancelled && modalResult.Data != null)
            {
                var item = (WorkspaceDto)modalResult.Data;
                if (item != null)
                {
                    TableService.Objects.Add(item);
                    StateHasChanged();
                }
            }

            Modal.OnClose -= AddResult;
        }

        private void Update(WorkspaceDto workspace)
        {
            var parameters = new OverlayModalParameters();
            parameters.Add(SD.Workspace, workspace);

            Modal.OnClose += EditResult;
            Modal.Show<Edit>(Text.WorkspaceEdit, parameters);
        }
        private void EditResult(OverlayModalResult modalResult)
        {
            if(!modalResult.Cancelled && modalResult.Data != null)
            {
                var item = (WorkspaceDto)modalResult.Data;
                
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
