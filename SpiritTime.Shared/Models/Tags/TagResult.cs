using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Models.Tags
{
    public class TagResult : ResultModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkspaceId { get; set; }
    }
}
