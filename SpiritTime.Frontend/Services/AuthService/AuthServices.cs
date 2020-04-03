﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.Registration;
using SpiritTime.Shared.Helper;

namespace SpiritTime.Frontend.Services.AuthService
{
    public class AuthServices : IAuthServices
    {
        public HttpClient _httpClient { get; }
        public AppSettings _appSettings { get; }
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        public AuthServices(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            httpClient.BaseAddress = new Uri(_appSettings.BookStoresBaseAddress);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");

            _httpClient = httpClient;
        }

        public async Task<AuthenticationResult> LoginAsync(AuthenticationResource user)
        {

            try
            {
                var result = await _httpClient.PostJsonAsync<AuthenticationResult>(Paths.LoginPath, user);

                if (result.Successful)
                {
                    ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(user.Email);
                    ((ApiAuthenticationStateProvider)_authenticationStateProvider).StateChanged();
                    //await _localStorage.SetItemAsync("authToken", result.Token);
                    //((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(user.Email);
                    //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
                    //((ApiAuthenticationStateProvider)_authenticationStateProvider).StateChanged();
                }

                return result;
            }
            catch (Exception ex)
            {
                return new AuthenticationResult { Successful = false, Error = ex.Message };
            }

            //user.Password = Crypt.Encrypt(user.Password);
            //string serializedUser = JsonConvert.SerializeObject(user);

            //var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/Login");
            //requestMessage.Content = new StringContent(serializedUser);

            //requestMessage.Content.Headers.ContentType
            //    = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            //var response = await _httpClient.SendAsync(requestMessage);

            //var responseStatusCode = response.StatusCode;
            //var responseBody = await response.Content.ReadAsStringAsync();

            //var returnedUser = JsonConvert.DeserializeObject<AuthenticationResult>(responseBody);

            //return await Task.FromResult(returnedUser);

        }

        public async Task<RegisterResult> RegisterUserAsync(RegisterResource user)
        {
            user.Password = Crypt.Encrypt(user.Password);
            string serializedUser = JsonConvert.SerializeObject(user);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/RegisterUser");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<RegisterResult>(responseBody);

            return await Task.FromResult(returnedUser);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(AuthenticationResource refreshRequest)
        {
            string serializedUser = JsonConvert.SerializeObject(refreshRequest);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/RefreshToken");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<AuthenticationResult>(responseBody);

            return await Task.FromResult(returnedUser);
        }

        public async Task<AuthenticationResult> GetUserByAccessTokenAsync(string accessToken)
        {
            string serializedRefreshRequest = JsonConvert.SerializeObject(accessToken);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/GetUserByAccessToken");
            requestMessage.Content = new StringContent(serializedRefreshRequest);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<AuthenticationResult>(responseBody);

            return await Task.FromResult(returnedUser);
        }



        public async Task Logout()
        {
            await ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
