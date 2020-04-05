using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiritTime.Shared.Models.WorkspaceModels
{
    public class WorkspaceResultIncludeTasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Task> Tasks { get; set; }
    }
}