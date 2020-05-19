using SpiritTime.Core.Contracts;
using SpiritTime.Core.Entities;

namespace SpiritTime.Persistence.Repositories
{
    public class ApplicationUserRepository: Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        
        public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}