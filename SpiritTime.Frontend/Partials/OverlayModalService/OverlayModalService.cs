using System;
using Microsoft.AspNetCore.Components;

namespace SpiritTime.Frontend.Partials.OverlayModalService
{
    public class OverlayModalService : IOverlayModalService
    {
        public event Action<OverlayModalResult> OnClose;
        internal event Action CloseModal;
        internal event Action<string, RenderFragment, OverlayModalParameters, OverlayModalOptions> OnShow;
        private Type _modalType;
        public void Show<T>(string title, OverlayModalParameters parameters) where T : ComponentBase
        {
            Show<T>(title, parameters, new OverlayModalOptions());
        }

        public void Show<T>(string title, OverlayModalParameters parameters = null, OverlayModalOptions options = null) where T : ComponentBase
        {
            Show(typeof(T), title, parameters, options);
        }

        public void Show(Type contentComponent, string title, OverlayModalParameters parameters, OverlayModalOptions options)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException("Must be a Blazor Component");
            }

            var content = new RenderFragment(x => { x.OpenComponent(1, contentComponent); x.CloseComponent(); });
            _modalType = contentComponent;
            OnShow.Invoke(title, content, parameters, options);
        }

        public void Close(OverlayModalResult modalResult)
        {
            modalResult.ModalType = _modalType;
            CloseModal?.Invoke();
            OnClose?.Invoke(modalResult);
        }

        public void Cancel()
        {
            CloseModal?.Invoke();
            OnClose?.Invoke(OverlayModalResult.Cancel(_modalType));
        }
    }
}
