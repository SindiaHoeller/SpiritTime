using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiritTime.Core;
using SpiritTime.Core.Contracts;
using SpiritTime.Persistence.Repositories;

namespace SpiritTime.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private bool _disposed;

        public ITaskRepository TaskRepository { get; set; }
        public ITagRepository TagRepository { get; }
        public ITaskTagRuleRepository TaskTagRuleRepository { get; }
        public IWorkspaceRepository WorkspaceRepository { get; }
        public ITaskTagRepository TaskTagRepository { get;  }
        public IApplicationUserRepository ApplicationUserRepository { get;  }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            TaskRepository = new TaskRepository(_dbContext);
            TagRepository = new TagRepository(_dbContext);
            TaskTagRuleRepository = new TaskTagRuleRepository(_dbContext);
            TaskTagRepository = new TaskTagRepository(_dbContext);
            WorkspaceRepository = new WorkspaceRepository(_dbContext);
            ApplicationUserRepository = new ApplicationUserRepository(_dbContext);
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _dbContext.Dispose();
        }
    }
}
