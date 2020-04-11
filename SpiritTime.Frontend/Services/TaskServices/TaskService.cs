using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Services.TaskServices
{
    public class TaskService : ServiceBase<TaskDto>, ITaskService
    {
        private readonly Paths _path;

        public TaskService(HttpClient httpClient,
            IOptions<AppSettings> appSettings,
            ILocalStorageService localStorageService)
            : base(httpClient, appSettings, localStorageService)
        {
            var appSetting = appSettings.Value;
            _path = new Paths(appSetting.BackendBaseAddress);
            EditPath = _path.TaskEdit;
            DeletePath = _path.TaskDelete;
        }
        public async Task<TaskListResult> GetAllAsync(int workspaceId)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.GetJsonAsync<TaskListResult>(_path.TaskGetAllByWorkspace + "?id=" + workspaceId);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new TaskListResult { Error = e.Message, Successful = false };
            }
        }
        
        public async Task<TaskListResult> GetByIdAsync(int id)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.GetJsonAsync<TaskListResult>(_path.TaskGetById + "?id=" + id);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new TaskListResult { Error = e.Message, Successful = false };
            }
        }

        public async Task<TaskResult> Add(TaskDto item)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.PostJsonAsync<TaskResult>(_path.TaskAdd, item);
            }
            catch (Exception ex)
            {
                return new TaskResult { Error = ex.Message, Successful = false };
            }
        }
    }
}