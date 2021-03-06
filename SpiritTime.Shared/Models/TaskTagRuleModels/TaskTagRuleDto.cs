namespace SpiritTime.Shared.Models.TaskTagRuleModels
{
    public class TaskTagRuleDto
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string TriggerText { get; set; }
        public bool TriggerName { get; set; }
        public bool TriggerDescription { get; set; }
        public bool ReplaceTrigger { get; set; }
    }
}