using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace SpiritTime.Frontend.Services.OverlayModalService
{
    public interface IOverlayModalService
    {
        event Action<OverlayModalResult> OnClose;
        void Show<T>(string title, OverlayModalParameters parameters) where T : ComponentBase;
        void Show<T>(string title, OverlayModalParameters parameters = null, OverlayModalOptions options = null) where T : ComponentBase;
        void Show(Type contentComponent, string title, OverlayModalParameters parameters, OverlayModalOptions options);


        void Close(OverlayModalResult modalResult);
        void Cancel();
    }
}
