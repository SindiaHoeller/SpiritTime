using SpiritTime.Core.Contracts;
using SpiritTime.Core.Entities;

namespace SpiritTime.Persistence.Repositories
{
    public class TaskTagRuleRepository : Repository<TaskTagRules>, ITaskTagRuleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TaskTagRuleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}