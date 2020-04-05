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
        [CascadingParameter] BaseOverlay Modal { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }

        bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        private WorkspaceDto Workspace { get; set; }
        public string Error { get; set; }

        protected override void OnInitialized()
        {
            Workspace = Parameters.Get<WorkspaceDto>(SD.Workspace);
            Modal.SetTitle(Text.AddWorkspace);
        }
        async void SubmitForm()
        {
            ShowForm = false;
            if (!String.IsNullOrEmpty(Workspace.Name))
            {
                Error = ErrorMsg.NameCanNotBeEmpty;
                ShowErrorForm = true;
            }
            else
            {
                var itemResource = _mapper.Map<WorkspaceResource>(Workspace);
                var item = await Service.Edit(itemResource);
                if (item.Successful)
                {
                    Workspace = _mapper.Map<WorkspaceDto>(item);
                }
                else
                {
                    Error = item.Error;
                    ShowErrorForm = true;
                }
                if (Workspace == null)
                {
                    Error = ErrorMsg.WorkspaceNotEdited;
                    ShowErrorForm = true;
                }
            }
        }

        void Done()
        {
            ModalService.Close(OverlayModalResult.Ok(Workspace));

        }

        void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
