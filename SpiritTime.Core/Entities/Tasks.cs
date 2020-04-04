using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritTime.Core.Entities
{
    public class Tasks : EntityObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBooked { get; set; }
        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<TaskTag> TaskTags { get; set; }
    }
}
