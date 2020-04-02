using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiritTime.Core.Contracts;
using SpiritTime.Core.Entities;

namespace SpiritTime.Persistence.Repositories
{
    public class TaskRepository : Repository<Tasks>, ITaskRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TaskRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
