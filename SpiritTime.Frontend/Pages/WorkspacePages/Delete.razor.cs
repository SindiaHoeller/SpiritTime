﻿using System;
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
    public partial class Delete
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] BaseOverlay Modal { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }
        [Inject] private IWorkspaceService Service { get; set; }

        bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        public string Error { get; set; }
        public int Id { get; set; }
        protected override void OnInitialized()
        {
            Id = Parameters.TryGet<int>(SD.Id);
            Modal.SetTitle(Text.ConfirmDeletion);
        }
        private async void SubmitForm()
        {
            
            var item = await Service.Delete(Id);
            if (!item.Successful)
            {
                ShowForm = false;
                Error = item.Error;
                ShowErrorForm = true;
            }
            else
            {
                ShowForm = false;
            }
            this.StateHasChanged();
        }

        private void Done()
        {
            ModalService.Close(OverlayModalResult.Ok(Id));

        }

        private void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
