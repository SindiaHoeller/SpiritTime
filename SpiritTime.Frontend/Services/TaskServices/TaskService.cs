using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Config;
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Models;
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
        public async Task<TaskListResult> GetAllAsync()
        {
            try
            {
                await SetAuthenticationHeader();
                var workspaceId = await GetCurrentWorkspaceId();
                return await _httpClient.GetJsonAsync<TaskListResult>(_path.TaskGetAllByWorkspace + "?id=" + workspaceId);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new TaskListResult { Error = e.Message, Successful = false };
            }
        }
        
        public async Task<TaskResult> GetCurrentTask()
        {
            try
            {
                await SetAuthenticationHeader();
                var workspaceId = await GetCurrentWorkspaceId();
                return await _httpClient.GetJsonAsync<TaskResult>(_path.TaskGetCurrentTaskByWorkspaceId + "?id=" + workspaceId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new TaskResult { Error = e.Message, Successful = false };
            }
        }
        
        public new async Task<TaskResult> Edit(TaskDto item)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.PostJsonAsync<TaskResult>(EditPath, item);
            }
            catch (Exception ex)
            {
                return new TaskResult {Error = ex.Message, Successful = false};
            }
        }
        
        public async Task<ResultModel> UpdateTags(TaskDto item)
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.PostJsonAsync<ResultModel>(_path.TaskUpdateTags, item);
            }
            catch (Exception ex)
            {
                return new ResultModel {Error = ex.Message, Successful = false};
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
                var newItem = new TaskNew
                {
                    Name = item.Name,
                    Description = item.Description,
                    IsBooked = false,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.MinValue,
                    WorkspaceId = await GetCurrentWorkspaceId(),
                    TagList = item.TagList
                };
                return await _httpClient.PostJsonAsync<TaskResult>(_path.TaskAdd, newItem);
            }
            catch (Exception ex)
            {
                return new TaskResult { Error = ex.Message, Successful = false };
            }
        }
        
        public async Task<(List<TaskDailyList>, ResultModel)> GetTaskDailyList(int lastDaysCount)
        {
            try
            {
                await SetAuthenticationHeader();
                var workspaceId = await GetCurrentWorkspaceId();
                var result =  await _httpClient.GetJsonAsync<TaskListResult>(_path.TaskGetAllByWorkspace + "?id=" + workspaceId);
                if (result.Successful)
                {
                    var dailyList = FullyPrepareList(result.ItemList);
                    return (dailyList, new ResultModel{Successful = true});
                }
                return (new List<TaskDailyList>(), new ResultModel{Successful = false, Error = result.Error});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (new List<TaskDailyList>(), new ResultModel{Successful = false, Error = e.Message});
            }

        }

        public List<TaskDailyList> FullyPrepareList(List<TaskDto> itemList)
        {
            var grouped = itemList
                .GroupBy(x => x.StartDate.ToShortDateString())
                .Select(x => x.ToList())
                .ToList();
                    
                    
            var dailyList = new List<TaskDailyList>();

            foreach (var list in grouped)
            {
                dailyList.Add(new TaskDailyList
                {
                    Date     = list.Select(x=>x.StartDate).FirstOrDefault(),
                    ItemList = list.OrderByDescending(x=>x.StartDate).ToList()
                });
            }

            return dailyList;
        }
    }
}