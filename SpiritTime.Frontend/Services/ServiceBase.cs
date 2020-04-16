using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Config;
using SpiritTime.Shared.Models;

namespace SpiritTime.Frontend.Services
{
    public class ServiceBase<T> : ServiceHelper where T : class
    {
        protected string EditPath { get; set; }
        protected string DeletePath { get; set; }

        protected ServiceBase(HttpClient httpClient, IOptions<AppSettings> appSettings,
            ILocalStorageService localStorageService) : base(httpClient, appSettings, localStorageService)
        {
            // var appSetting = appSettings.Value;
            // var path = new Paths(appSetting.BackendBaseAddress);
            //_httpClient.BaseAddress = new Uri(appSetting.BackendBaseAddress);
            //_httpClient.DefaultRequestHeaders.Add(SD.UserAgent, SD.BlazorServer);
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
                return new ResultModel {Error = ex.Message, Successful = false};
            }
        }

        public async Task<ResultModel> Delete(int id)
        {
            await SetAuthenticationHeader();

            try
            {
                var path = DeletePath + "?id=" + id;
                return await _httpClient.GetJsonAsync<ResultModel>(path);
            }
            catch (Exception ex)
            {
                return new ResultModel {Error = ex.Message, Successful = false};
            }
        }
    }
}