using System.Collections.Generic;
using System.Threading.Tasks;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Services.TaskServices
{
    public interface ITaskService : IServiceBase
    {
        Task<TaskListResult> GetAllAsync();
        Task<TaskListResult> GetByIdAsync(int id);
        Task<(List<TaskDailyList>, ResultModel)> GetTaskDailyList(int lastDaysCount);
        Task<ResultModel> UpdateTags(TaskDto item);
        Task<TaskResult> Add(TaskDto item);
        Task<ResultModel> Edit(TaskDto item);
        Task<ResultModel> Delete(int id);
        List<TaskDailyList> FullyPrepareList(List<TaskDto> itemList);
    }
}