using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiritTime.Shared.Models.Workspace
{
    public class WorkspaceResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Task> Tasks { get; set; }
    }
}