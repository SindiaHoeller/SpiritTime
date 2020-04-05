using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SpiritTime.Core.Entities;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services.WorkspaceServices
{
    public class WorkspaceService : ServiceBase, IWorkspaceService
    {
        private Paths Path;        

        public WorkspaceService(HttpClient httpClient, IOptions<AppSettings> appSettings, ILocalStorageService localStorageService)
            : base(httpClient, appSettings, localStorageService)
        {
            var appSetting = appSettings.Value;
            Path = new Paths(appSetting.BackendBaseAddress);
        }

        public async Task<WorkspaceListResult> GetAllAsync()
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.GetJsonAsync<WorkspaceListResult>(Path.WorkspaceGetAll);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new WorkspaceListResult { Error = e.Message, Successful = false };
            }            
        }

        
    }
}