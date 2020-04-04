using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Services.AuthServices;
using SpiritTime.Shared.Models.Account.Registration;

namespace SpiritTime.Frontend.Pages.Account
{
    public partial class Register
    {
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        private RegisterResource RegisterResource = new RegisterResource();
        private bool ShowErrors;
        private string Error;
        private string ConfirmPassword;

        private async Task HandleRegistration()
        {
            ShowErrors = false;
            if (RegisterResource.Password != ConfirmPassword)
            {
                Error = "Die beiden eingegebenen Passwörter stimmen nicht überein. Überprüfen Sie ihre Eingabe.";
                ShowErrors = true;
            }
            else
            {
                var result = await AuthService.RegisterUserAsync(RegisterResource);

                if (result.Successful)
                {
                    NavigationManager.NavigateTo("/login");
                }
                else
                {
                    Error = result.Error;
                    ShowErrors = true;
                }
            }
        }
    }
}
