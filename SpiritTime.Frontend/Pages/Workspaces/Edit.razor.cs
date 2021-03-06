using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.Overlays;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Workspaces
{
    public partial class Edit
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [CascadingParameter] private BaseOverlay Modal { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }

        bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        private WorkspaceDto Workspace { get; set; }
        public string Error { get; set; }

        protected override void OnInitialized()
        {
            Workspace = Parameters.TryGet<WorkspaceDto>(SD.Workspace);
            Modal.SetTitle(TextMsg.WorkspaceEdit);
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
                    ToastService.ShowSuccess(SuccessMsg.WorkspaceEdited + Workspace.Name);
                    ModalService.Close(OverlayModalResult.Ok(Workspace));
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
            StateHasChanged();
        }
        private void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
