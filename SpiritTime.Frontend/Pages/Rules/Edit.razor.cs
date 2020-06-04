using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.Overlays;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.TaskTagRuleServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Frontend.Pages.Rules
{
    public partial class Edit
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] private BaseOverlay Modal { get; set; }
        [Inject] private ITaskTagRuleService Service { get; set; }
        [Inject] private ITagService TagService { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }
        [Inject] private IToastService ToastService { get; set; }

        private bool ShowForm { get; set; }
        private bool ShowErrorForm { get; set; } = false;
        private TaskTagRuleDto Item { get; set; }
        private string Error { get; set; }
        private List<TagDto> TagList { get; set; }
        private string Id = string.Empty;

        protected override async void OnInitialized()
        {
            var result = await TagService.GetAllAsync();
            TagList = result.Successful ? result.ItemList : new List<TagDto>();
            Item = Parameters.TryGet<TaskTagRuleDto>(SD.Item);
            Id = Item.TagId.ToString();
            Modal.SetTitle(TextMsg.TagRuleEdit);
            ShowForm = true;
            StateHasChanged();
        }
        private async void SubmitForm()
        {
            Int32.TryParse(Id, out int id);
            Item.TagId = id;
            ShowForm = false;
            if (String.IsNullOrEmpty(Item.TriggerText))
            {
                ToastService.ShowError(ErrorMsg.NameCanNotBeEmpty);
            }
            else if (Item.TagId <= 0)
            {
                ToastService.ShowError(ErrorMsg.ChooseOption);
            }
            else
            {
                var result = await Service.Edit(Item);
                if (result.Successful)
                {
                    ToastService.ShowSuccess(SuccessMsg.RuleEdited + Item.TriggerName);
                    ModalService.Close(OverlayModalResult.Ok(Item));
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
        private void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}