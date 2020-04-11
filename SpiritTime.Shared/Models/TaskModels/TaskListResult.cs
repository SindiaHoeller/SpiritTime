using System.Collections.Generic;

namespace SpiritTime.Shared.Models.TaskModels
{
    public class TaskListResult : ResultModel
    {
        public List<TaskDto> ItemList { get; set; }
    }
}