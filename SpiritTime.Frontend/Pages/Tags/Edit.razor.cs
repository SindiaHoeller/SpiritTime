using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Pages.Overlays;
using SpiritTime.Frontend.Services.OverlayModalService;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.Tags;

namespace SpiritTime.Frontend.Pages.Tags
{
    public partial class Edit
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] private BaseOverlay Modal { get; set; }
        [Inject] private ITagService Service { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }

        private bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        private TagDto Item { get; set; }
        public string Error { get; set; }

        protected override void OnInitialized()
        {
            Item = Parameters.TryGet<TagDto>(SD.Item);
            Modal.SetTitle(Text.WorkspaceEdit);
        }
        private async void SubmitForm()
        {
            ShowForm = false;
            if (string.IsNullOrEmpty(Item.Name))
            {
                Error = ErrorMsg.NameCanNotBeEmpty;
                ShowErrorForm = true;
            }
            else
            {
                var itemResource = _mapper.Map<TagResource>(Item);
                var result = await Service.Edit(itemResource);
                if (result.Successful)
                {
                    Item = _mapper.Map<TagDto>(itemResource);
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
            this.StateHasChanged();
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
