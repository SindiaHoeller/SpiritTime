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
        [Inject] private ITableService<Workspace> TableService { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetAllAsync();
            if (result.Successful)
            {
                TableService.Objects = result.Workspaces;
            }

        }
    }
}
