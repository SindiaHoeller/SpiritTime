using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.AuthServices;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.Account;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.ChangeUserPassword;

namespace SpiritTime.Frontend.Pages.Account
{
    public partial class Overview
    {
        [Inject] private IAuthService Service      { get; set; }
        [Inject] private IToastService  ToastService { get; set; }
        private UserInfo UserInfo { get; set; }
        private bool ShowError { get; set; }
        private string ErrorMessage { get; set; }
        public ChangeUserPasswordResource PWChangeInfo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetUserInfo();
            if (result.Successful)
            {
                UserInfo = result.UserInfo;
                PWChangeInfo.Email = UserInfo.Email;
            }
            else
            {
                ShowError = true;
                ErrorMessage = result.Error;
            }
        }

        private async Task SaveChanges()
        {
            var result = await Service.UpdateUserInfo(UserInfo);
            if (!result.Successful)
            {
                ToastService.ShowError(result.Error);
            }
            else
            {
                ToastService.ShowSuccess(SuccessMsg.UpdatedUserInfo);
            }
        }

        private async Task ChangePassword()
        {
            if (string.IsNullOrEmpty(PWChangeInfo.Password))
            {
                ToastService.ShowError(ErrorMsg.PasswordDoesNotMeetRequirements);
            }
            else if (PWChangeInfo.Password != PWChangeInfo.NewPassword)
            {
                ToastService.ShowError(ErrorMsg.PasswordDoNotMatch);
            }
            else
            {
                // TODO
            }
        }
    }
}