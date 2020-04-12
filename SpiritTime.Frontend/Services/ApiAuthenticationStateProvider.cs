//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Blazored.LocalStorage;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Http;
//using SpiritTime.Core.Entities;
//using SpiritTime.Frontend.Services.AuthService;
//using SpiritTime.Shared.Models.Account.Authentication;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using SpiritTime.Shared.Models.Account.Authentication;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        //public IAuthServices _userService { get; set; }
        private readonly HttpClient _httpClient;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorageService,
            //IAuthServices userService,
            HttpClient httpClient)
        {
            //throw new Exception("CustomAuthenticationStateProviderException");
            _localStorage = localStorageService;
            //_userService = userService;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await _localStorage.GetItemAsync<string>(SD.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SD.Bearer, accessToken);

            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(JwtHelper.ParseClaimsFromJwt(accessToken), "jwt")));
        }

        public async Task MarkUserAsAuthenticated(string token, string email)
        {
            await _localStorage.SetItemAsync(SD.AccessToken, token);
            await _localStorage.SetItemAsync(SD.RefreshToken, token);

            var authenticatedUser =
                new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, email)}, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SD.Bearer, token);
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync(SD.RefreshToken);
            await _localStorage.RemoveItemAsync(SD.AccessToken);

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }


        public void MarkUserAsAuthenticated(string email)
        {
            var authenticatedUser =
                new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, email)}, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<User> GetCurrentUser()
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            var emailClaim = user.Claims.Where(x => x.Type == "email").FirstOrDefault();
            var idClaim = user.Claims.Where(x => x.Type == "nameid").FirstOrDefault();
            return new User
            {
                Email = emailClaim.Value,
                Id = Int32.Parse(idClaim.Value),
            };
        }

        public void StateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); // <- Does nothing
        }


    }
}