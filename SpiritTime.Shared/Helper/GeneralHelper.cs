using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Shared.Helper
{
    public static class GeneralHelper
    {
        public static void TrimTask(TaskDto item)
        {
            item.Name = item.Name.Trim();
            item.Description = item.Description.Trim();
        }
        public static void TrimTask(Tasks item)
        {
            item.Name = item.Name.Trim();
            item.Description = item.Description.Trim();
        }
    }
}