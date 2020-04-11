using SpiritTime.Core.Contracts;
using SpiritTime.Core.Entities;

namespace SpiritTime.Persistence.Repositories
{
    public class TaskTagRepository : Repository<TaskTag>, ITaskTagRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TaskTagRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}