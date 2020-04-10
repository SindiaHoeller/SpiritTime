using System.Threading.Tasks;
using SpiritTime.Core.Entities;

namespace SpiritTime.Core.Contracts
{
    public interface IWorkspaceRepository : IRepository<Workspace>
    {
        Task<string> GetUserIdByWorkspaceId(int workspaceId);
    }
}