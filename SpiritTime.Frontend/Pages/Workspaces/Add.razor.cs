using System;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.Overlays;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Workspaces
{
    public partial class Add
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }
        [CascadingParameter] BaseOverlay Modal { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }
        [Inject] private IMapper _mapper { get; set; }

        bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        private string Name { get; set; }
        private WorkspaceDto Workspace { get; set; }
        public string Error { get; set; }

        protected override void OnInitialized()
        {
            Modal.SetTitle(Text.AddWorkspace);
        }
        async void SubmitForm()
        {
            ShowForm = false;
            if (String.IsNullOrEmpty(Name))
            {
                Error = ErrorMsg.NameCanNotBeEmpty;
                ShowErrorForm = true;
            }
            else
            {
                var item = await Service.Add(Name);
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
                    Error = ErrorMsg.WorkspaceNotAdded;
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
