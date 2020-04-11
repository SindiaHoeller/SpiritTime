using System;
using System.Collections.Generic;
using SpiritTime.Shared.Models.TagModels;

namespace SpiritTime.Shared.Models.TaskModels
{
    public class TaskNew
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBooked { get; set; }
        public int WorkspaceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TagInfo> TagList { get; set; }
    }
}