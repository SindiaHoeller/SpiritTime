using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Models.Tags
{
    public class TagListResult : ResultModel
    {
        public List<TagDto> ItemList { get; set; }
    }
}
