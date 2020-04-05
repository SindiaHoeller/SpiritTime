using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Core.Entities;
using SpiritTime.Frontend.Services.TableServices;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.WorkspacePages
{
    public partial class Overview
    {
        [Inject] private ITableService<WorkspaceDto> TableService { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }
        public bool ShowError { get; set; } = false;
        public string ErrorMsg { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetAllAsync();
            if (result.Successful)
            {
                TableService.Objects = result.Workspaces;
            }
            else
            {
                ErrorMsg = result.Error;
                ShowError = true;
            }
        }

        private async Task Delete(int id)
        {

        }
        private async Task Add()
        {

        }
        private async Task Update(WorkspaceDto workspace)
        {

        }

    }
}
