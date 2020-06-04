using System.Threading.Tasks;
using SpiritTime.Shared.Models;

namespace SpiritTime.Frontend.Services.OptionsService
{
    public interface IOptionService
    {
        Task<WorkspaceOption> GetCurrentWorkspaceAndList();
        Task<ResultModel> SetWorkspace(int id);
    }
}