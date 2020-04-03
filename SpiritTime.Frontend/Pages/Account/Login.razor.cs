using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Services.AuthService;
using SpiritTime.Shared.Models.Account.Authentication;

namespace SpiritTime.Frontend.Pages.Account
{
    public partial class Login
    {
        [Inject] private IAuthServices AuthService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private AuthenticationResource loginModel = new AuthenticationResource();
        private bool ShowErrors;
        private string Error = "";

        private async Task HandleLogin()
        {
            ShowErrors = false;

            var result = await AuthService.LoginAsync(loginModel);
            

            if (result.Successful)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                Error = result.Error;
                ShowErrors = true;
            }
        }
    }
}
