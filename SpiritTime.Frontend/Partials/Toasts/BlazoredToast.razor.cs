using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.ToastModal;

namespace SpiritTime.Frontend.Partials.Toasts
{
    public partial class BlazoredToast
    {
        [CascadingParameter] private BlazoredToasts ToastsContainer { get; set; }
        [Parameter] public Guid ToastId { get; set; }
        [Parameter] public ToastSettings ToastSettings { get; set; }

        private void Close()
        {
            ToastsContainer.RemoveToast(ToastId);
        }
    }
}