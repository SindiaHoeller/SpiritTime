namespace SpiritTime.Core.Entities
{
    public class TaskTagRule : EntityObject
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public string TriggerText { get; set; }
        public bool TriggerName { get; set; }
        public bool TriggerDescription { get; set; }
    }
}