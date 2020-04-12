using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Services.OptionsService;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Options
{
    public partial class Overview
    {
        [Inject] private IOptionService Service { get; set; }
        private bool ShowError { get; set; }
        private string ErrMsg { get; set; }
        private bool ShowWorkspaces { get; set; }
        public List<WorkspaceDto> WorkspaceList { get; set; }
        public int CurrentWorkspaceId { get; set; }
        public string CurrentWorkspaceStringId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetCurrentWorkspaceAndList();
            if (result.Success)
            {
                WorkspaceList = result.WorkspaceList;
                CurrentWorkspaceId = result.CurrentWorkspaceId;
                CurrentWorkspaceStringId = result.CurrentWorkspaceId.ToString();
                ShowWorkspaces = true;
            }
            else
            {
                ErrMsg = result.ErrorMsg;
                ShowError = true;
            }
        }

        private void SetWorkspace()
        {
            Int32.TryParse(CurrentWorkspaceStringId, out int id);
            if (id > 0)
            {
                Service.SetWorkspace(id);
            }
            else
            {
                ErrMsg = ErrorMsg.ChooseOption;
                ShowError = true;
            }
            StateHasChanged();
        }
    }
}