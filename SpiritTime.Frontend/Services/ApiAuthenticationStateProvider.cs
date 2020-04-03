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
using SpiritTime.Frontend.Services.AuthService;
using SpiritTime.Shared.Models.Account.Authentication;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Services.StaticDetails;

namespace SpiritTime.Frontend.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        public ILocalStorageService _localStorage { get; }
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

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(accessToken), "jwt")));
        }

        public async Task MarkUserAsAuthenticated(string token, string email)
        {
            await _localStorage.SetItemAsync(SD.AccessToken, token);
            await _localStorage.SetItemAsync(SD.RefreshToken, token);

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "apiauth"));
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

        //private ClaimsPrincipal GetClaimsIdentity(AuthenticationResult user)
        //{
        //    var claimsIdentity = new ClaimsIdentity();
        //    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.) }, "apiauth"));

        //    //if (user.Email != null)
        //    //{
        //    //    claimsIdentity = new ClaimsIdentity(new[]
        //    //    {
        //    //        new Claim(ClaimTypes.Name, user.EmailAddress),
        //    //        new Claim(ClaimTypes.Role, user.Role.RoleDesc),
        //    //        new Claim("IsUserEmployedBefore1990", IsUserEmployedBefore1990(user))
        //    //    }, "apiauth_type");
        //    //}

        //    return authenticatedUser;
        //}


        public void MarkUserAsAuthenticated(string email)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "apiauth"));
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

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }
        public void StateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); // <- Does nothing
        }


        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
