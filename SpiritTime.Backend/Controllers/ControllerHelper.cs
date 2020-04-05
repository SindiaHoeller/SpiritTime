using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritTime.Shared.Helper;

namespace SpiritTime.Backend.Controllers
{
    public class ControllerHelper : ControllerBase 
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ControllerHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        

        protected bool CheckForPermissionById(string suggestedUserId)
        {
            var realUserId = GetUserId();
            return suggestedUserId == realUserId;
        }

        protected string GetUserId()
        {
            return JwtHelper.GetUserIdByToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
        }
    }
}