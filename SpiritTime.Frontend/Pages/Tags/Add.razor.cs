using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Pages.Overlays;
using SpiritTime.Frontend.Services.OverlayModalService;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.Tags;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Tags
{
    public partial class Add
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }
        [CascadingParameter] BaseOverlay Modal { get; set; }
        [Inject] private ITagService Service { get; set; }
        [Inject] private IWorkspaceService WorkspaceService { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        
        bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        private string Name { get; set; }
        private TagDto Item { get; set; }
        public string Error { get; set; }
        public List<WorkspaceDto> WorkspaceList { get; set; }

        protected override async void OnInitialized()
        {
            var result = await WorkspaceService.GetAllAsync();
            WorkspaceList = result.Successful ? result.Workspaces : new List<WorkspaceDto>() ;
            Modal.SetTitle(Text.TagAdd);
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
                    Item = _mapper.Map<TagDto>(item);
                }
                else
                {
                    Error = item.Error;
                    ShowErrorForm = true;
                }

                if (Item == null)
                {
                    Error = ErrorMsg.WorkspaceNotAdded;
                    ShowErrorForm = true;
                }
            }
        }

        void Done()
        {
            ModalService.Close(OverlayModalResult.Ok(Item));

        }

        void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
