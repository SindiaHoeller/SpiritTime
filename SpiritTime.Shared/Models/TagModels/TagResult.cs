namespace SpiritTime.Shared.Models.TagModels
{
    public class TagResult : ResultModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
        public int WorkspaceId { get; set; }
        public string WorkspaceName { get; set; }
    }
}
