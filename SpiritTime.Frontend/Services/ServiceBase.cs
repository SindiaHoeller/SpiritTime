using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Data;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.Tags;

namespace SpiritTime.Frontend.Services
{
    public class ServiceBase<T> where T : class
    {
        public HttpClient _httpClient { get; }
        public ILocalStorageService _localStorageService { get; }
        public string EditPath { get; set; }
        public string DeletePath { get; set; }
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
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                var token = await _localStorageService.GetItemAsync<string>(SD.AccessToken);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SD.Bearer, token);
            }
        }

        public async Task<ResultModel> Edit(T item)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.PostJsonAsync<ResultModel>(EditPath, item);
            }
            catch (Exception ex)
            {
                return new ResultModel { Error = ex.Message, Successful = false };
            }
        }
        public async Task<ResultModel> Delete(int id)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.PostJsonAsync<ResultModel>(DeletePath, id.ToString());
            }
            catch (Exception ex)
            {
                return new ResultModel { Error = ex.Message, Successful = false };
            }
        }
    }
}
