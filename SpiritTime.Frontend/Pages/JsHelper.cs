using System.Threading.Tasks;
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
    }
}