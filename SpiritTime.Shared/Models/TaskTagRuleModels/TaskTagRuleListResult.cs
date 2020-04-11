using System.Collections.Generic;

namespace SpiritTime.Shared.Models.TaskTagRuleModels
{
    public class TaskTagRuleListResult: ResultModel
    {
        public List<TaskTagRuleDto> ItemList { get; set; }
    }
}