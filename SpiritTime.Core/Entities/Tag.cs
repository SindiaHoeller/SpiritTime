using System.Collections.Generic;

namespace SpiritTime.Core.Entities
{
    public class Tag : EntityObject
    {
        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }

        public ICollection<TaskTag> TaskTags { get; set; }
    }
}