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
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IMapper _mapper;
        public WorkspacesController(JwtAuthentication jwtAuthentication, 
            ILogger<WorkspacesController> logger,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork, IMapper mapper) : base(httpContextAccessor)
        {
            _logger = logger;
            _jwtAuthentication = jwtAuthentication;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        ///     Get's all Workspaces for current User
        /// </summary>
        /// <remarks> Needs: nothing <br />  Returns: WorkspaceListResult </remarks>
        /// <param name="GetallByUserId"></param>
        /// <returns></returns>
        /// <response code="200">Returns a WorkspaceListResult</response>

        [HttpGet]
        public async Task<IActionResult> GetallByUserId()
        {
            try
            {
                var userId = GetUserId();
                List<Workspace> workspaceList = await _unitOfWork.WorkspaceRepository.GetMultipleByAsync(x => x.UserId == userId);
                var list = _mapper.Map<List<WorkspaceDto>>(workspaceList);
                //return new JsonResult(list);
                return new JsonResult(new WorkspaceListResult{Workspaces = list, Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new WorkspaceListResult{Error = ex.Message, Successful = false});
            }
        }

        /// <summary>
        ///     Creates a new Workspace
        /// </summary>
        /// <remarks>Needs: WorkspaceResourceNew <br /> Returns: WorkspaceResult</remarks>
        /// <param name="Create"></param>
        /// <returns></returns>
        /// <response code="200">Returns a WorkspaceResult</response>
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
                var newWorkspace = await _unitOfWork.WorkspaceRepository.GetUniqueByAsync(x => x.Name == workspaceResource.Name && x.UserId == userId);
                var workspaceResult = _mapper.Map<WorkspaceResult>(newWorkspace);
                workspaceResult.Successful = true;

                return new JsonResult(workspaceResult);
            }
            catch (Exception ex)
            {
                return new JsonResult(new WorkspaceResult{Error = ex.Message, Successful = false});
            }
        }

        /// <summary>
        ///     Updates a Workspace
        /// </summary>
        /// <remarks>Needs: WorkspaceResource <br /> Returns: ResultModel</remarks>
        /// <param name="Update"></param>
        /// <returns></returns>
        /// <response code="200">Returns a ResultModel</response>
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
        ///     Deletes a Workspace
        /// </summary>
        /// <remarks>Needs: Id \
        /// Returns: ResultModel</remarks>
        /// <param name="Delete"></param>
        /// <returns></returns>
        /// <response code="200">Returns a ResultModel</response>
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