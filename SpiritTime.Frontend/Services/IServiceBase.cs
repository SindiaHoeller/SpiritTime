using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services
{
    public interface IServiceBase
    {
        Task<string> GetLocalStorageByKey(string key);
        Task SetLocalStorageByKey(string key, string value);
    }
}