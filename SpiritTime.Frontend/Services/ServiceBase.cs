using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Data;
using SpiritTime.Shared.Helper;

namespace SpiritTime.Frontend.Services
{
    public class ServiceBase
    {
        public HttpClient _httpClient { get; }
        public ILocalStorageService _localStorageService { get; }
        public ServiceBase(HttpClient httpClient, IOptions<AppSettings> appSettings, ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;

            var appSetting = appSettings.Value;
            _httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri(appSetting.BackendBaseAddress);
            //_httpClient.DefaultRequestHeaders.Add(SD.UserAgent, SD.BlazorServer);
            
        }


        protected async Task SetAuthenticationHeader()
        {
            var token = await _localStorageService.GetItemAsync<string>(SD.AccessToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SD.Bearer, token);

        }
    }
}
