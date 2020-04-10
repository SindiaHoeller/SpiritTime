using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpiritTime.Core.Contracts;
using SpiritTime.Core.Entities;

namespace SpiritTime.Persistence.Repositories
{
    public class TagRepository: Repository<Tag>, ITagRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TagRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}