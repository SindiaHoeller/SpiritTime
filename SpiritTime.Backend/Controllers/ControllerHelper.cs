using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritTime.Core;
using SpiritTime.Shared.Helper;

namespace SpiritTime.Backend.Controllers
{
    /// <summary>
    /// ControllerHelper
    /// </summary>
    public class ControllerHelper : ControllerBase 
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// ControllerHelper
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public ControllerHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }



        /// <summary>
        /// CheckForPermissionById
        /// </summary>
        /// <param name="suggestedUserId"></param>
        /// <returns></returns>
        protected bool CheckForPermissionById(string suggestedUserId)
        {
            var realUserId = GetUserId();
            return suggestedUserId == realUserId;
        }

        /// <summary>
        /// CheckForPermissionByWorkspace
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        protected async Task<bool> CheckForPermissionByWorkspace(int workspaceId, IUnitOfWork unitOfWork)
        {
            var userId =
                await unitOfWork.WorkspaceRepository.GetUserIdByWorkspaceId(workspaceId);

            return CheckForPermissionById(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        protected async Task<bool> CheckForPermissionByTag(int tagId, IUnitOfWork unitOfWork)
        {
            var item = await unitOfWork.TagRepository.GetUniqueByAsync(x => x.Id == tagId);
            return await CheckForPermissionByWorkspace(item.WorkspaceId, unitOfWork);
        }

        /// <summary>
        /// GetUserId
        /// </summary>
        /// <returns></returns>
        protected string GetUserId()
        {
            return JwtHelper.GetUserIdByToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
        }
    }
}