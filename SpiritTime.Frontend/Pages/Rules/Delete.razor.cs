using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.Overlays;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.TaskTagRuleServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;

namespace SpiritTime.Frontend.Pages.Rules
{
    public partial class Delete
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [Inject] private ITaskTagRuleService Service { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [CascadingParameter] BaseOverlay Modal { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }
        

        bool ShowForm { get; set; } = true;
        private bool ShowErrorForm { get; set; } = false;
        public string Error { get; set; }
        public int Id { get; set; }
        
        
        protected override void OnInitialized()
        {
            Id = Parameters.TryGet<int>(SD.Id);
            Modal.SetTitle(TextMsg.ConfirmDeletion);
        }
        private async void SubmitForm()
        {

            var item = await Service.Delete(Id);
            ShowForm = false;
            if (!item.Successful)
            {
                Error = item.Error;
                ShowErrorForm = true;
            }
            else
            {
                ToastService.ShowSuccess(SuccessMsg.SuccessedDeletion);
                ModalService.Close(OverlayModalResult.Ok(Id));
            }
            StateHasChanged();
        }

        private void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}