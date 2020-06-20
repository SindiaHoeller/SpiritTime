using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SpiritTime.Frontend.Pages
{
    public static class JsHelper
    {
        public static async Task Focus(this ElementReference elementRef, IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeVoidAsync(
                "jsInterop.focusElement", elementRef);
        }

        public static async Task CloseWindow(IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeVoidAsync("jsInterop.closeWindow");
        }

    }
}