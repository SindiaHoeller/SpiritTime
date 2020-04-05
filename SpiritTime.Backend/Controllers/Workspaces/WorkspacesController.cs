using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpiritTime.Backend.Infrastructure.Jwt;
using SpiritTime.Core;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Backend.Controllers.Workspaces
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkspacesController : ControllerHelper 
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<WorkspacesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public WorkspacesController(JwtAuthentication jwtAuthentication, 
            ILogger<WorkspacesController> logger,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork) : base(httpContextAccessor)
        {
            _logger = logger;
            _jwtAuthentication = jwtAuthentication;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     Get's all Workspaces for current User
        /// </summary>
        /// <param name="GetallByUserId"></param>
        /// <returns></returns>
        /// <response code="200">Returns a JsonWebToken</response>
        /// <response code="400">Invalid credentials</response>
        
        [HttpGet]
        public async Task<IActionResult> GetallByUserId()
        {
            try
            {
                var userId = GetUserId();
                var workspaceList = await _unitOfWork.WorkspaceRepository.GetMultipleByAsync(x => x.UserId == userId);
                return new JsonResult(new WorkspaceListResult{Workspaces = workspaceList, Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new WorkspaceListResult{Error = ex.Message, Successful = false});
            }
        }
        
        /// <summary>
        ///     Get's all Workspaces for one User
        /// </summary>
        /// <param name="Create"></param>
        /// <returns></returns>
        /// <response code="200">Returns a JsonWebToken</response>
        /// <response code="400">Invalid credentials</response>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(WorkspaceResourceNew workspaceResource)
        {
            try
            {
                var userId = GetUserId();
                var workspace = new Workspace {Name = workspaceResource.Name, UserId = userId};
                await _unitOfWork.WorkspaceRepository.AddAsync(workspace);
                await _unitOfWork.SaveAsync();
                return new JsonResult(new ResultModel{Error = null, Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel{Error = ex.Message, Successful = false});
            }
        }
        
        /// <summary>
        ///     Get's all Workspaces for one User
        /// </summary>
        /// <param name="Update"></param>
        /// <returns></returns>
        /// <response code="200">Returns a JsonWebToken</response>
        /// <response code="400">Invalid credentials</response>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(WorkspaceResource workspaceResource)
        {
            try
            {
                var workspace = await _unitOfWork.WorkspaceRepository
                    .GetUniqueByAsync(x => x.Id == workspaceResource.Id);

                if (!CheckForPermissionById(workspace.UserId))
                    return new JsonResult(new ResultModel
                        {Error = ErrorMsg.NotAuthorizedForAction, Successful = false});
                
                workspace.Name = workspaceResource.Name;
                _unitOfWork.WorkspaceRepository.Update(workspace);
                await _unitOfWork.SaveAsync();
                return new JsonResult(new ResultModel{Error = null, Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel{Error = ex.Message, Successful = false});
            }
        }
        
        
        /// <summary>
        ///     Get's all Workspaces for one User
        /// </summary>
        /// <param name="Delete"></param>
        /// <returns></returns>
        /// <response code="200">Returns a JsonWebToken</response>
        /// <response code="400">Invalid credentials</response>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var workspace = await _unitOfWork.WorkspaceRepository
                    .GetUniqueByAsync(x => x.Id == id);

                if (!CheckForPermissionById(workspace.UserId))
                    return new JsonResult(new ResultModel
                        {Error = ErrorMsg.NotAuthorizedForAction, Successful = false});
                
                _unitOfWork.WorkspaceRepository.Remove(workspace);
                await _unitOfWork.SaveAsync();
                return new JsonResult(new ResultModel{Error = null, Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel{Error = ex.Message, Successful = false});
            }
        }
        
    }
}