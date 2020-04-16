using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritTime.Shared.Models.TaskModels
{
    public class TaskDailyList
    {
        public List<TaskDto> ItemList { get; set; }
        public DateTime Date { get; set; }

        public TimeSpan TimeSpan =>
            ItemList
                .Where(x=>x.EndDate != DateTime.MinValue)
                .Select(x=>x.EndDate.Subtract(x.StartDate))
                .Aggregate(TimeSpan.Zero,(sumSoFar, nextTimeSpan) => sumSoFar + nextTimeSpan.Duration());

        public string TimeSpanText { get; set; }
    }
}