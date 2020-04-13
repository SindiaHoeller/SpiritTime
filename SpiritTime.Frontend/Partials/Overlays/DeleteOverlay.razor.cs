using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Shared.Messages;

namespace SpiritTime.Frontend.Partials.Overlays
{
    public partial class DeleteOverlay
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }
        [CascadingParameter] BaseOverlay BaseOverlay { get; set; }

        bool ShowForm { get; set; } = true;
        public bool ShowErrorForm { get; set; } = false;
        public int Id { get; set; }

        protected override void OnInitialized()
        {
            Id = Parameters.Get<int>("Id");
            BaseOverlay.SetTitle(TextMsg.ConfirmDeletion);

        }
        void SubmitForm()
        {
            ModalService.Close(OverlayModalResult.Ok(Id));
        }

        void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
