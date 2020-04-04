using System.Collections.Generic;

namespace SpiritTime.Core.Entities
{
    public class Workspace : EntityObject
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Tasks> Tasks { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}