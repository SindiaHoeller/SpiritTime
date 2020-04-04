using SpiritTime.Core.Contracts;
using SpiritTime.Core.Entities;

namespace SpiritTime.Persistence.Repositories
{
    public class TagRuleRepository : Repository<TaskTagRule>, ITagRuleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TagRuleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}