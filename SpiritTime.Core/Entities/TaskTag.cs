namespace SpiritTime.Core.Entities
{
    public class TaskTag: EntityObject
    {
        public int TaskId { get; set; }
        public Tasks Tasks { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}