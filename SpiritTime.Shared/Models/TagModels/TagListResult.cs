using System.Collections.Generic;

namespace SpiritTime.Shared.Models.TagModels
{
    public class TagListResult : ResultModel
    {
        public List<TagDto> ItemList { get; set; }
    }
}
