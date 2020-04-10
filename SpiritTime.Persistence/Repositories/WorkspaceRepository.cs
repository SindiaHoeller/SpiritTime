using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpiritTime.Core.Contracts;
using SpiritTime.Core.Entities;

namespace SpiritTime.Persistence.Repositories
{
    public class WorkspaceRepository : Repository<Workspace>, IWorkspaceRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public WorkspaceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetUserIdByWorkspaceId(int workspaceId)
        {
            var item = await _dbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId);
            return item.UserId;
        }
    }
}