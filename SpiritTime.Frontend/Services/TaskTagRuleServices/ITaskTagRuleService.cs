using System.Threading.Tasks;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Frontend.Services.TaskTagRuleServices
{
    public interface ITaskTagRuleService
    {
        Task<TaskTagRuleListResult> GetAllAsync();
        Task<TaskTagRuleResult> Add(TaskTagRuleNew item);
        Task<ResultModel> Edit(TaskTagRuleDto item);
        Task<ResultModel> Delete(int id);
    }
}