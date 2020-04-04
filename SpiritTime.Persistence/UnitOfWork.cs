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
        public ITagRuleRepository TagRuleRepository { get; }
        public IWorkspaceRepository WorkspaceRepository { get; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            TaskRepository = new TaskRepository(_dbContext);
            TagRepository = new TagRepository(_dbContext);
            TagRuleRepository = new TagRuleRepository(_dbContext);
            WorkspaceRepository = new WorkspaceRepository(_dbContext);
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
