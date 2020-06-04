using System;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;

namespace SpiritTime.Frontend.Partials.Overlays
{
    public partial class ConfirmOverlay
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] public OverlayModalParameters Parameters { get; set; }
        [CascadingParameter] BaseOverlay BaseOverlay { get; set; }

        bool ShowForm { get; set; } = true;
        public bool ShowErrorForm { get; set; } = false;
        public int Id { get; set; }

        protected override void OnInitialized()
        {

            Id = Parameters.Get<int>("Id");
            Console.WriteLine(Id);
        }
        void SubmitForm()
        {
            Parameters = new OverlayModalParameters();
            ModalService.Close(OverlayModalResult.Ok(Id));
        }

        void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
