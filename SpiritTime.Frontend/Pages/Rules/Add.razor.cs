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
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Frontend.Pages.Rules
{
    public partial class Add
    {
        [Inject] private     IOverlayModalService   ModalService { get; set; }
        [CascadingParameter] BaseOverlay            Modal        { get; set; }
        [Inject] private     ITaskTagRuleService    Service      { get; set; }
        [Inject] private     IMapper                Mapper       { get; set; }
        [Inject] private     ITagService            TagService   { get; set; }
        [Inject] private     IToastService          ToastService { get; set; }

        private bool           ShowForm        { get; set; }
        private bool           ShowErrorForm   { get; set; }
        private bool           ShowSuccessForm { get; set; }
        private TaskTagRuleDto Item            { get; set; }
        private string         Error           { get; set; }
        private string         Id              { get; set; } = string.Empty;
        private List<TagDto>   TagList         { get; set; }

        protected override async void OnInitialized()
        {
            var result = await TagService.GetAllAsync();
            TagList = result.Successful ? result.ItemList : new List<TagDto>();
            Id      = TagList.FirstOrDefault()?.Id.ToString();
            Item    = new TaskTagRuleDto();
            Modal.SetTitle(TextMsg.TagRuleAdd);
            ShowForm = true;
            StateHasChanged();
        }

        private async void SubmitForm()
        {
            ShowForm = false;
            Int32.TryParse(Id, out int id);
            Item.TagId = id;
            if (String.IsNullOrEmpty(Item.TriggerText))
            {
                Error = ErrorMsg.NameCanNotBeEmpty;
                ShowErrorForm = true;
            }
            else if (Item.TagId <= 0)
            {
                Error         = ErrorMsg.ChooseOption;
                ShowErrorForm = true;
            }
            else
            {
                var newItem = Mapper.Map<TaskTagRuleNew>(Item);
                var item    = await Service.Add(newItem);
                if (item.Successful)
                {
                    Item            = item.Item;
                    ToastService.ShowSuccess(SuccessMsg.RuleAdded + Item.TriggerName);
                    ModalService.Close(OverlayModalResult.Ok(Item));
                }
                else
                {
                    Error         = item.Error;
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