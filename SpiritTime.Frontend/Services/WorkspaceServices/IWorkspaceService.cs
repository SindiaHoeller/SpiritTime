using System.Threading.Tasks;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services.WorkspaceServices
{
    public interface IWorkspaceService
    {
        Task<WorkspaceListResult> GetAllAsync();
        Task<WorkspaceResult> Add(string name);
        Task<ResultModel> Edit(WorkspaceResource workspace);
        Task<ResultModel> Delete(int id);
    }
}