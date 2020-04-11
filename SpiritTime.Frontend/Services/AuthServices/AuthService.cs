using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.Registration;

namespace SpiritTime.Frontend.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        public HttpClient _httpClient { get; }
        //public AppSettings _appSettings { get; }
        private Paths Path;
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public AuthService(HttpClient httpClient, IOptions<AppSettings> appSettings, AuthenticationStateProvider provider)
        {
            var appSetting = appSettings.Value;
            Path = new Paths(appSetting.BackendBaseAddress);
            httpClient.BaseAddress = new Uri(appSetting.BackendBaseAddress);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");
            
            _authenticationStateProvider = provider;
            _httpClient = httpClient;
        }

        public async Task<AuthenticationResult> LoginAsync(AuthenticationResource user)
        {

            try
            {
                var result = await _httpClient.PostJsonAsync<AuthenticationResult>(Path.Login, user);

                if (result.Successful)
                {
                    await ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token, user.Email);
                    ((ApiAuthenticationStateProvider)_authenticationStateProvider).StateChanged();
                    await ((ApiAuthenticationStateProvider) _authenticationStateProvider).SetCurrentWorkspace(0,
                        Path.WorkspaceGetOne);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new AuthenticationResult { Successful = false, Error = ex.Message };
            }
        }



        public async Task<RegisterResult> RegisterUserAsync(RegisterResource user)
        {
            try
            {
                var result = await _httpClient.PostJsonAsync<RegisterResult>(Path.Register, user);
                return result;
            }
            catch (Exception ex)
            {
                return new RegisterResult { Successful = false, Error = ex.Message };
            }
        }

        //public async Task<AuthenticationResult> RefreshTokenAsync(AuthenticationResource refreshRequest)
        //{
        //    string serializedUser = JsonConvert.SerializeObject(refreshRequest);

        //    var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/RefreshToken");
        //    requestMessage.Content = new StringContent(serializedUser);

        //    requestMessage.Content.Headers.ContentType
        //        = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        //    var response = await _httpClient.SendAsync(requestMessage);

        //    var responseStatusCode = response.StatusCode;
        //    var responseBody = await response.Content.ReadAsStringAsync();

        //    var returnedUser = JsonConvert.DeserializeObject<AuthenticationResult>(responseBody);

        //    return await Task.FromResult(returnedUser);
        //}

        //public async Task<AuthenticationResult> GetUserByAccessTokenAsync(string accessToken)
        //{
        //    string serializedRefreshRequest = JsonConvert.SerializeObject(accessToken);

        //    var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/GetUserByAccessToken");
        //    requestMessage.Content = new StringContent(serializedRefreshRequest);

        //    requestMessage.Content.Headers.ContentType
        //        = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        //    var response = await _httpClient.SendAsync(requestMessage);

        //    var responseStatusCode = response.StatusCode;
        //    var responseBody = await response.Content.ReadAsStringAsync();

        //    var returnedUser = JsonConvert.DeserializeObject<AuthenticationResult>(responseBody);

        //    return await Task.FromResult(returnedUser);
        //}



        public async Task Logout()
        {
            await ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
