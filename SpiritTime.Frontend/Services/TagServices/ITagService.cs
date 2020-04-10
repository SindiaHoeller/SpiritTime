using System.Threading.Tasks;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.Tags;

namespace SpiritTime.Frontend.Services.TagServices
{
    public interface ITagService
    {
        Task<TagListResult> GetAllAsync();
        Task<TagResult> Add(string name);
        Task<ResultModel> Edit(TagResource item);
        Task<ResultModel> Delete(int id);
    }
}