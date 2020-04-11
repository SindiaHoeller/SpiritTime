using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Pages.Overlays;
using SpiritTime.Frontend.Services.OverlayModalService;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Tags
{
    public partial class Add
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] BaseOverlay Modal { get; set; }
        [Inject] private ITagService Service { get; set; }
        [Inject] private IWorkspaceService WorkspaceService { get; set; }
        [Inject] private IMapper _mapper { get; set; }

        private bool ShowForm { get; set; }
        private bool ShowErrorForm { get; set; }
        private bool ShowSuccessForm { get; set; }
        private TagDto Item { get; set; }
        private string Error { get; set; }
        private string Id = string.Empty;
        private List<WorkspaceDto> WorkspaceList { get; set; }

        protected override async void OnInitialized()
        {
            var result = await WorkspaceService.GetAllAsync();
            WorkspaceList = result.Successful ? result.Workspaces : new List<WorkspaceDto>();
            Item = new TagDto();
            Modal.SetTitle(Text.TagAdd);
            ShowForm = true;
            StateHasChanged();
        }

        async void SubmitForm()
        {
            Int32.TryParse(Id, out int id);
            Item.WorkspaceId = id;
            ShowForm = false;
            if (String.IsNullOrEmpty(Item.Name))
            {
                Error = ErrorMsg.NameCanNotBeEmpty;
                ShowErrorForm = true;
            }
            else if (Item.WorkspaceId <= 0)
            {
                Error = ErrorMsg.WorkspaceChoose;
                ShowErrorForm = true;
            }
            else
            {
                var item = await Service.Add(Item);
                if (item.Successful)
                {
                    Item = _mapper.Map<TagDto>(item);
                    ShowSuccessForm = true;
                }
                else
                {
                    Error = item.Error;
                    ShowErrorForm = true;
                }

                if (Item == null)
                {
                    Error = ErrorMsg.TagNotAdded;
                    ShowErrorForm = true;
                }
            }

            StateHasChanged();
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