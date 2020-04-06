using System;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Pages.Overlays;
using SpiritTime.Frontend.Services.OverlayModalService;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.WorkspacePages
{
    public partial class Edit
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] private BaseOverlay Modal { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }

        bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        private WorkspaceDto Workspace { get; set; }
        public string Error { get; set; }

        protected override void OnInitialized()
        {
            Workspace = Parameters.TryGet<WorkspaceDto>(SD.Workspace);
            Modal.SetTitle(Text.WorkspaceEdit);
        }
        private async void SubmitForm()
        {
            ShowForm = false;
            if (string.IsNullOrEmpty(Workspace.Name))
            {
                Error = ErrorMsg.NameCanNotBeEmpty;
                ShowErrorForm = true;
            }
            else
            {
                var itemResource = _mapper.Map<WorkspaceResource>(Workspace);
                var result = await Service.Edit(itemResource);
                if (result.Successful)
                {
                    Workspace = _mapper.Map<WorkspaceDto>(itemResource);
                }
                else
                {
                    Error = result.Error;
                    ShowErrorForm = true;
                }
                if (Workspace == null)
                {
                    Error = ErrorMsg.WorkspaceNotEdited;
                    ShowErrorForm = true;
                }
            }
            this.StateHasChanged();
        }

        private void Done()
        {
            ModalService.Close(OverlayModalResult.Ok(Workspace));

        }
        private void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
