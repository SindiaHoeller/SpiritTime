using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Shared.Helper
{
    public static class GeneralHelper
    {
        public static void TrimTask(TaskDto item)
        {
            if(!string.IsNullOrEmpty(item.Name))
                item.Name = item.Name.Trim();
            if(!string.IsNullOrEmpty(item.Description))
                item.Description = item.Description.Trim();
        }
        public static void TrimTask(Tasks item)
        {
            if(!string.IsNullOrEmpty(item.Name))
                item.Name = item.Name.Trim();
            if(!string.IsNullOrEmpty(item.Description))
                item.Description = item.Description.Trim();
        }
    }
}