using System;
using System.Collections.Generic;

namespace SpiritTime.Shared.Models.TaskModels
{
    public class TaskDailyList
    {
        public List<TaskDto> ItemList { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}