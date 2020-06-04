using System.Threading.Tasks;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.TagModels;

namespace SpiritTime.Frontend.Services.TagServices
{
    public interface ITagService
    {
        Task<TagListResult> GetAllAsync();
        Task<TagResult> Add(TagDto item);
        Task<ResultModel> Edit(TagResource item);
        Task<ResultModel> Delete(int id);
    }
}