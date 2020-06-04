using AutoMapper;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;

namespace SpiritTime.Frontend.Partials.Overlays
{
    public partial class AddModel : ComponentBase
    {
        [Inject] protected IMapper _mapper { get; set; }
        [Inject] protected IOverlayModalService ModalService { get; set; }
        [CascadingParameter] protected OverlayModalParameters Parameters { get; set; }
        [CascadingParameter] protected BaseOverlay Modal { get; set; }
        protected bool ShowForm { get; set; }
        protected bool ShowErrorForm { get; set; } = false;
        protected bool ShowSuccessForm { get; set; }
        protected string Error { get; set; }
        protected object Item { get; set; }
        
        protected void Done()
        {
            ModalService.Close(OverlayModalResult.Ok(Item));
        }

        protected void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}