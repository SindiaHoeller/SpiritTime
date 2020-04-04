using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpiritTime.Backend.Infrastructure.Jwt;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.Workspace;

namespace SpiritTime.Backend.Controllers.Workspace
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WorkspaceController : ControllerBase
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<WorkspaceController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WorkspaceController(JwtAuthentication jwtAuthentication, 
            ILogger<WorkspaceController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _jwtAuthentication = jwtAuthentication;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///     Changes the user password.
        /// </summary>
        /// <param name="Getall"></param>
        /// <returns></returns>
        /// <response code="200">Password successfully changed</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="409">Password rules are broken</response>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var returnInfo = new List<string>();
            returnInfo.Add(_httpContextAccessor.HttpContext.User.Identity.Name);
            returnInfo.Add(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
            returnInfo.Add(_httpContextAccessor.HttpContext.Response.Headers["Authorization"]);
            returnInfo.Add(_httpContextAccessor.HttpContext.Request.Headers[SD.Bearer]);
            returnInfo.Add(_httpContextAccessor.HttpContext.Response.Headers[SD.Bearer]);
            
            
            
            return new JsonResult(returnInfo);
            //return new JsonResult(new WorkspaceResult{Name = value, Id = 12});
        }
    }
}