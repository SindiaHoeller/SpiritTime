using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.Overlays;
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
            Id = WorkspaceList.FirstOrDefault()?.Id.ToString();
            Item = new TagDto();
            Modal.SetTitle(TextMsg.TagAdd);
            ShowForm = true;
            StateHasChanged();
        }

        private async void SubmitForm()
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
                    Item = item.Item;
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