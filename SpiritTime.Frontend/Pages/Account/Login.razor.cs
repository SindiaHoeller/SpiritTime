using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Services;
using SpiritTime.Frontend.Services.AuthServices;
using SpiritTime.Shared.Models.Account.Authentication;

namespace SpiritTime.Frontend.Pages.Account
{
    public partial class Login
    {
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationProvider { get; set; }
        private AuthenticationResource LoginModel { get; set; }
        private bool ShowErrors;
        private string Error = "";

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("Login page loaded...");
            LoginModel = new AuthenticationResource();
            var authenticated = await AuthenticationProvider.GetAuthenticationStateAsync();
            if (authenticated.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/");
            }
        }
        private async Task HandleLogin()
        {
            ShowErrors = false;
            Console.WriteLine("HandleLogin activated...");
            var result = await AuthService.LoginAsync(LoginModel);
            Console.WriteLine(result.Error);
            if(!result.Successful)
            {
                Error = result.Error;
                ShowErrors = true;
            }
            else
            {
                Console.WriteLine("Navigate to Main page...");
                NavigationManager.NavigateTo("/");
            }
        }

    }
}
