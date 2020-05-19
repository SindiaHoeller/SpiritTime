using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.AuthServices;
using SpiritTime.Shared.Models.Account;
using SpiritTime.Shared.Models.Account.Authentication;

namespace SpiritTime.Frontend.Pages.Account
{
    public partial class Overview
    {
        [Inject] private IAuthService Service      { get; set; }
        [Inject] private IToastService  ToastService { get; set; }
        private UserInfo UserInfo { get; set; }
        private bool ShowError { get; set; }
        private string ErrorMsg { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetUserInfo();
            if (result.Successful)
            {
                UserInfo = result.UserInfo;
            }
            else
            {
                ShowError = true;
                ErrorMsg = result.Error;
            }
        }

        private void SaveChanges()
        {
            
        }
    }
}