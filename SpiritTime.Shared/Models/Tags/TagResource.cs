using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Models.Tags
{
    public class TagResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkspaceId { get; set; }
        public string WorkspaceName { get; set; }
    }
}
