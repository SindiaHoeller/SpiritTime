using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Services.OverlayModalService;

namespace SpiritTime.Frontend.Pages.Overlays
{
    public partial class BaseOverlay : IDisposable
    {
        const string _defaultStyle = "blazored-modal";
        const string _defaultPosition = "blazored-modal-center";

        [Inject]
        private IOverlayModalService ModalService { get; set; }
        [Parameter]
        public bool HideHeader { get; set; }
        [Parameter]
        public bool HideCloseButton { get; set; }
        [Parameter]
        public bool DisableBackgroundCancel { get; set; }
        [Parameter]
        public string Position { get; set; }
        [Parameter]
        public string Style { get; set; }

        public bool ComponentDiesableBackgroundCancel { get; set; }
        public bool ComponentHideHeader { get; set; }
        public bool ComponentHideCloseButton { get; set; }
        public string ComponentPosition { get; set; }
        public string ComponentStyle { get; set; }
        public bool IsVisible { get; set; }
        public string Title { get; set; }
        public RenderFragment Content { get; set; }
        public OverlayModalParameters Parameters { get; set; }

        protected override void OnInitialized()
        {
            ((OverlayModalService)ModalService).OnShow += ShowModal;
            ((OverlayModalService)ModalService).CloseModal += CloseModal;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async void ShowModal(string title, RenderFragment content, OverlayModalParameters parameters, OverlayModalOptions options)
        {
            Title = title;
            Content = content;
            Parameters = parameters;
            IsVisible = true;
            await InvokeAsync(StateHasChanged);

        }
        public async void CloseModal()
        {
            Title = "";
            Content = null;
            ComponentStyle = "";
            IsVisible = false;
            await InvokeAsync(StateHasChanged);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((OverlayModalService)ModalService).OnShow -= ShowModal;
                ((OverlayModalService)ModalService).CloseModal -= CloseModal;
            }
        }

        public void HandelBackgroundClick()
        {
            if (ComponentDiesableBackgroundCancel) return;
            ModalService.Cancel();
        }

        public void SetModalOptions(OverlayModalOptions options)
        {
            ComponentHideHeader = HideHeader;
            if (options.HideHeader.HasValue)
                ComponentHideHeader = options.HideHeader.Value;

            ComponentHideCloseButton = HideCloseButton;
            if (options.HideCloseButton.HasValue)
                ComponentHideCloseButton = options.HideCloseButton.Value;

            ComponentDiesableBackgroundCancel = DisableBackgroundCancel;
            if (options.DisableBackgroundCancel.HasValue)
                ComponentDiesableBackgroundCancel = options.DisableBackgroundCancel.Value;

            ComponentPosition = string.IsNullOrWhiteSpace(options.Position) ? Position : options.Position;
            if (string.IsNullOrWhiteSpace(ComponentPosition))
                ComponentPosition = _defaultPosition;

            ComponentStyle = string.IsNullOrWhiteSpace(options.Style) ? Style : options.Style;
            if (string.IsNullOrWhiteSpace(ComponentStyle))
                ComponentStyle = _defaultStyle;
        }

        public void SetTitle(string title)
        {
            Title = title;
            StateHasChanged();
        }
    }
}
