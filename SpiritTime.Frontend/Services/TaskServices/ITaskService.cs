using System.Threading.Tasks;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Services.TaskServices
{
    public interface ITaskService
    {
        Task<TaskListResult> GetAllAsync(int workspaceId);
        Task<TaskListResult> GetByIdAsync(int id);
        Task<TaskResult> Add(TaskDto item);
        Task<ResultModel> Edit(TaskDto item);
        Task<ResultModel> Delete(int id);
    }
}