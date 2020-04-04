using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.Workspace;

namespace SpiritTime.Frontend.Services.WorkspaceServices
{
    public class WorkspaceService : IWorkspaceService
    {
        public HttpClient _httpClient { get; }
        public ILocalStorageService _localStorageService { get; }
        private Paths Path;
        

        public WorkspaceService(HttpClient httpClient, IOptions<AppSettings> appSettings, ILocalStorageService localStorageService)
        {
            var appSetting = appSettings.Value;
            Path = new Paths(appSetting.BackendBaseAddress);
            
            httpClient.BaseAddress = new Uri(appSetting.BackendBaseAddress);
            httpClient.DefaultRequestHeaders.Add(SD.UserAgent, SD.BlazorServer);
            
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }


        private async Task SetAuthenticationHeader()
        {
            var token = await _localStorageService.GetItemAsync<string>(SD.AccessToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SD.Bearer, token);
            
        }

        public async Task<bool> GetAllAsync()
        {
            await SetAuthenticationHeader();

            try
            {
                //var resu = await _httpClient.PostAsync(Path.WorkspaceGetAll, null);
                var result = await _httpClient.GetJsonAsync<WorkspaceResult>(Path.WorkspaceGetAll);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}