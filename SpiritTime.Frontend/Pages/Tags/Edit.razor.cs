using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Pages.Overlays;
using SpiritTime.Frontend.Services.OverlayModalService;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.WorkspaceServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.Tags;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Tags
{
    public partial class Edit
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] private BaseOverlay Modal { get; set; }
        [Inject] private ITagService Service { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        [Inject] private IWorkspaceService WorkspaceService { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }

        private bool ShowForm { get; set; }
        private bool ShowErrorForm { get; set; } = false;
        private bool ShowSuccessForm { get; set; }
        private TagDto Item { get; set; }
        private string Error { get; set; }
        private List<WorkspaceDto> WorkspaceList { get; set; }
        private string Id = string.Empty;

        protected override async void OnInitialized()
        {
            var result = await WorkspaceService.GetAllAsync();
            WorkspaceList = result.Successful ? result.Workspaces : new List<WorkspaceDto>();
            Item = Parameters.TryGet<TagDto>(SD.Item);
            Id = Item.WorkspaceId.ToString();
            Modal.SetTitle(Text.WorkspaceEdit);
            ShowForm = true;
            StateHasChanged();
        }
        private async void SubmitForm()
        {
            Int32.TryParse(Id, out int id);
            Item.WorkspaceId = id;
            ShowForm = false;
            if (string.IsNullOrEmpty(Item.Name))
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
                var workspace = WorkspaceList.FirstOrDefault(x => x.Id == id);
                Item.WorkspaceName = workspace?.Name;
                var itemResource = _mapper.Map<TagResource>(Item);
                var result = await Service.Edit(itemResource);
                if (result.Successful)
                {
                    Item = _mapper.Map<TagDto>(itemResource);
                    ShowSuccessForm = true;
                }
                else
                {
                    Error = result.Error;
                    ShowErrorForm = true;
                }
                if (Item == null)
                {
                    Error = ErrorMsg.TagNotEdited;
                    ShowErrorForm = true;
                }
            }
            StateHasChanged();
        }

        private void Done()
        {
            ModalService.Close(OverlayModalResult.Ok(Item));

        }
        private void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
