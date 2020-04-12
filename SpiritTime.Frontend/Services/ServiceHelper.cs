using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Config;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services
{
    public class ServiceHelper
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILocalStorageService _localStorage;
        private string GetFirstOrDefaultWorkspacePath { get; }

        protected ServiceHelper(HttpClient httpClient, IOptions<AppSettings> appSettings, ILocalStorageService localStorageService)
        {
            var appSetting = appSettings.Value;
            _httpClient = httpClient;
            _localStorage = localStorageService;
            var path = new Paths(appSetting.BackendBaseAddress);
            GetFirstOrDefaultWorkspacePath = path.WorkspaceGetFirstOrDefault;
        }

        protected async Task SetCurrentWorkspace(int id = 0)
        {
            //var name = await _localStorage.GetItemAsync<string>(SD.CurrentWorkspace);
            if (id == 0)
            {
                var workspace = await _httpClient.GetJsonAsync<WorkspaceDto>(GetFirstOrDefaultWorkspacePath);
                id = workspace.Id;
            }

            await _localStorage.SetItemAsync(SD.CurrentWorkspace, id);
        }

        protected async Task<string> GetCurrentWorkspace()
        {
            return await _localStorage.GetItemAsync<string>(SD.CurrentWorkspace);
        }
        protected async Task<int> GetCurrentWorkspaceId()
        {
            var stringId = await _localStorage.GetItemAsync<string>(SD.CurrentWorkspace);
            var success = Int32.TryParse(stringId, out int id);
            return success ? id : 0;
        }
        protected async Task SetAuthenticationHeader()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                var token = await _localStorage.GetItemAsync<string>(SD.AccessToken);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SD.Bearer, token);
            }
        }
    }
}