using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiritTime.Core.Contracts;

namespace SpiritTime.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository TaskRepository { get; }
        ITagRepository TagRepository { get; }
        ITagRuleRepository TagRuleRepository { get; }
        IWorkspaceRepository WorkspaceRepository { get; }
        
        Task SaveAsync();
    }
}
