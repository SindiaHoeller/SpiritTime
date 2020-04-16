using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Config;
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services.WorkspaceServices
{
    public class WorkspaceService : ServiceBase<WorkspaceResource>, IWorkspaceService
    {
        private readonly Paths _path;        

        public WorkspaceService(HttpClient httpClient,
            IOptions<AppSettings> appSettings,
            ILocalStorageService localStorageService)
            : base(httpClient, appSettings, localStorageService)
        {
            var appSetting = appSettings.Value;
            _path = new Paths(appSetting.BackendBaseAddress);
            EditPath = _path.WorkspaceEdit;
            DeletePath = _path.WorkspaceDelete;
        }

        public async Task<WorkspaceListResult> GetAllAsync()
        {
            try
            {
                await SetAuthenticationHeader();
                return await _httpClient.GetJsonAsync<WorkspaceListResult>(_path.WorkspaceGetAll);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new WorkspaceListResult { Error = e.Message, Successful = false };
            }            
        }

        public async Task<WorkspaceResult> Add(string name)
        {
            await SetAuthenticationHeader();

            try
            {
                WorkspaceResourceNew workspace = new WorkspaceResourceNew { Name = name };
                return await _httpClient.PostJsonAsync<WorkspaceResult>(_path.WorkspaceAdd, workspace);
            }
            catch (Exception ex)
            {
                return new WorkspaceResult { Error = ex.Message, Successful = false };
            }
        }
    }
}