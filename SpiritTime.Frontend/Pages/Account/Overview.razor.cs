using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.AuthServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.Account;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.ChangeUserPassword;
using SpiritTime.Shared.Models.Account.DeleteUser;

namespace SpiritTime.Frontend.Pages.Account
{
    public partial class Overview
    {
        [Inject] private IAuthService Service      { get; set; }
        [Inject] private IToastService  ToastService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private UserInfo UserInfo { get; set; }
        private bool ShowError { get; set; }
        private bool ShowPasswordReset { get; set; }
        private string ErrorMessage { get; set; }
        [DisplayName("Password Confirmation")]
        private string PasswordConfirmation { get; set; }
        private ChangeUserPasswordResource PWChangeInfo { get; set; }
        private Alert DeleteAlert;
        private bool HideDeleteButton { get; set; }
        private string DeleteConfirmation { get; set; }
        

        protected override async Task OnInitializedAsync()
        {
            var result = await Service.GetUserInfo();
            PWChangeInfo = new ChangeUserPasswordResource();
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
            if (string.IsNullOrEmpty(PWChangeInfo.Password) | string.IsNullOrEmpty(PWChangeInfo.NewPassword))
            {
                ToastService.ShowError(ErrorMsg.PasswordDoesNotMeetRequirements);
            }
            else if (PasswordConfirmation != PWChangeInfo.NewPassword)
            {
                ToastService.ShowError(ErrorMsg.PasswordDoNotMatch);
            }
            else
            {
                var (isValid, errorMsg) = PasswordValidator.ByIdentityStandard(PWChangeInfo.NewPassword);
                if(!isValid)
                    ToastService.ShowError(errorMsg);
                else
                {
                    var result = await Service.ChangeUserPassword(PWChangeInfo);
                    if (result.Successful)
                    {
                        ToastService.ShowSuccess(SuccessMsg.PasswordUpdated);
                    }
                    else
                    {
                        ToastService.ShowError(string.Join(", ", result.Error));
                    }
                }

            }
        }

        private void ToggleDelete()
        {
            DeleteAlert.Toggle();
            HideDeleteButton = !HideDeleteButton;
            DeleteConfirmation = "";
            StateHasChanged();
        }

        private async Task DeleteOwnUser()
        {
            if (DeleteConfirmation == TextMsg.DeleteConfirmString)
            {
                var result = await Service.DeleteOwnUser(new DeleteUserResource{Email = UserInfo.Email});
                if (!result.Successful)
                {
                    ToastService.ShowError(string.Join(", ", result.Error));
                }
                else
                {
                    ToastService.ShowSuccess(SuccessMsg.UpdatedUserInfo);
                    await Service.Logout();
                    NavigationManager.NavigateTo("/");
                }
            }
            else
            {
                ToastService.ShowInfo(ErrorMsg.Abort);
                ToggleDelete();
            }
        }

        private void TogglePasswordReset()
        {
            ShowPasswordReset = !ShowPasswordReset;
        }
    }
}