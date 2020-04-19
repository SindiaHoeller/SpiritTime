using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.Overlays;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Frontend.Pages.Tasks
{
    public partial class TagSearch
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [CascadingParameter] BaseOverlay Modal { get; set; }
        [CascadingParameter] OverlayModalParameters Parameters { get; set; }
        private bool ShowForm { get; set; }
        private bool ShowErrorForm { get; set; }
        private bool ShowSuccessForm { get; set; }
        public List<TagDto> TagList { get; set; }
        public TagDto SelectedTag { get; set; }
        
        
        protected override async void OnInitialized()
        {
            TagList = Parameters.TryGet<List<TagDto>>(SD.ItemList);
            Modal.SetTitle(TextMsg.TagRuleAdd);
            ShowForm = true;
            StateHasChanged();
        }

        private async void SubmitForm()
        {
            if (!string.IsNullOrEmpty(SelectedTag.Name))
            {
                ShowSuccessForm = true;
            }
            else
            {
                ShowErrorForm = true;
            }
        }
        
        private async Task<IEnumerable<TagDto>> SearchTag(string searchText)
        {
            return await Task.FromResult(TagList.Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToList());
        }
        private void Done()
        {
            ModalService.Close(OverlayModalResult.Ok(TagList));

        }
        private void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
        private void SelectedTask()
        {
            ToastService.ShowInfo(SelectedTag.Name);
            // ToastService.ShowInfo(SelectedTag?.Name);
            // StartTimer();
        }

        private void OnFocusInStopTimer()
        {
            ToastService.ShowError("Stopped");
            // StopTimer();
        }
    }
}