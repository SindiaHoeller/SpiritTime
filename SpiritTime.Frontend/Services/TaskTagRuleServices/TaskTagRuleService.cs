using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Frontend.Services.TaskTagRuleServices
{
    public class TaskTagRuleService : ServiceBase<TaskTagRuleDto>, ITaskTagRuleService
    {
        private readonly Paths _path;

        public TaskTagRuleService(HttpClient httpClient,
            IOptions<AppSettings> appSettings,
            ILocalStorageService localStorageService)
            : base(httpClient, appSettings, localStorageService)
        {
            var appSetting = appSettings.Value;
            _path = new Paths(appSetting.BackendBaseAddress);
            EditPath = _path.TaskTagRuleEdit;
            DeletePath = _path.TaskTagRuleDelete;
        }
        
        public async Task<TaskTagRuleListResult> GetAllAsync()
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.GetJsonAsync<TaskTagRuleListResult>(_path.TaskTagRuleGetAll);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new TaskTagRuleListResult { Error = e.Message, Successful = false };
            }
        }

        public async Task<TaskTagRuleResult> Add(TaskTagRuleNew item)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.PostJsonAsync<TaskTagRuleResult>(_path.TaskTagRuleAdd, item);
            }
            catch (Exception ex)
            {
                return new TaskTagRuleResult { Error = ex.Message, Successful = false };
            }
        }
    }
}