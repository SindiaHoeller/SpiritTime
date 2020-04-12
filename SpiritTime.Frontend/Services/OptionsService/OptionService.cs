using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Config;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services.OptionsService
{
    public class OptionService : ServiceHelper, IOptionService
    {
        private readonly Paths _path;

        public OptionService(HttpClient httpClient,
            IOptions<AppSettings> appSettings,
            ILocalStorageService localStorageService) : base(httpClient, appSettings, localStorageService)
        {
            _path = new Paths(appSettings.Value.BackendBaseAddress);
        }

        public async Task SetCurrentWorkspaceOption(int id)
        {
            await SetCurrentWorkspace(id);
        }

        public async Task<WorkspaceOption> GetCurrentWorkspaceAndList()
        {
            try
            {
                await SetAuthenticationHeader();
                
                var result =  await _httpClient.GetJsonAsync<WorkspaceListResult>(_path.WorkspaceGetAll);
                if (result.Successful)
                {
                    var workspaceOption = new WorkspaceOption
                    {
                        Success = true,
                        WorkspaceList = result.Workspaces,
                        CurrentWorkspaceId = await GetCurrentWorkspaceId()
                    };
                    return workspaceOption;
                }
                else
                {
                    return new WorkspaceOption {Success = false, ErrorMsg = result.Error};
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new WorkspaceOption {Success = false, ErrorMsg = e.Message};
            }     
        }

        public async Task<ResultModel> SetWorkspace(int id)
        {
            try
            {
                await SetCurrentWorkspace(id);
                return new ResultModel{Successful = true};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResultModel{Successful = true, Error = e.Message};
            }
        }
    }
}