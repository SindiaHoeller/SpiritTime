using System.Threading.Tasks;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services.WorkspaceServices
{
    public interface IWorkspaceService
    {
        Task<WorkspaceListResult> GetAllAsync();
    }
}