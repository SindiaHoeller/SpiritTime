using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Data;
using SpiritTime.Frontend.Services.StaticDetails;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Services.TagServices
{
    public class TagService : ServiceBase<TagResource>, ITagService
    {
        private readonly Paths _path;

        public TagService(HttpClient httpClient,
            IOptions<AppSettings> appSettings,
            ILocalStorageService localStorageService)
            : base(httpClient, appSettings, localStorageService)
        {
            var appSetting = appSettings.Value;
            _path = new Paths(appSetting.BackendBaseAddress);
            EditPath = _path.TagEdit;
            DeletePath = _path.TagDelete;
        }

        public async Task<TagListResult> GetAllAsync()
        {
            await SetAuthenticationHeader();

            try
            {
                return await _httpClient.GetJsonAsync<TagListResult>(_path.TagGetAll);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new TagListResult { Error = e.Message, Successful = false };
            }
        }

        public async Task<TagResult> Add(TagDto item)
        {
            await SetAuthenticationHeader();

            try
            {
                TagResourceNew itemNew = new TagResourceNew { Name = item.Name, WorkspaceId = item.WorkspaceId};
                return await _httpClient.PostJsonAsync<TagResult>(_path.TagAdd, itemNew);
            }
            catch (Exception ex)
            {
                return new TagResult { Error = ex.Message, Successful = false };
            }
        }
    }
}