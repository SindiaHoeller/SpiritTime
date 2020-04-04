using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services.WorkspaceServices
{
    public interface IWorkspaceService
    {
        Task<bool> GetAllAsync();
    }
}