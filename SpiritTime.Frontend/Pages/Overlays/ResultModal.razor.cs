using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Services.OverlayModalService;

namespace SpiritTime.Frontend.Pages.Overlays
{
    public partial class ResultModal
    {
        [Inject] private IOverlayModalService ModalService { get; set; }
        [CascadingParameter] public OverlayModalParameters Parameters { get; set; }
        [CascadingParameter] BaseOverlay BaseOverlay { get; set; }

        bool ShowForm { get; set; } = true;
        public bool ShowErrorForm { get; set; } = false;
        public int Id { get; set; }
        public string Answer { get; set; }

        protected override void OnInitialized()
        {
            Id = Parameters.Get<int>("Id");
            Answer = Parameters.Get<string>("Answer");
            Console.WriteLine(Id);
        }
        void SubmitForm()
        {
            ModalService.Close(OverlayModalResult.Ok(Id));
        }

        void Cancel()
        {
            ModalService.Close(OverlayModalResult.Cancel());
        }
    }
}
