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
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Backend.Controllers.Workspaces
{
    /// <summary>
    /// WorkspaceController
    /// </summary>
    [ApiController]
    [Route(ControllerNames.Workspaces)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkspacesController : ControllerHelper
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<WorkspacesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        /// <summary>
        /// WorkspacesController
        /// </summary>
        /// <param name="jwtAuthentication"></param>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
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
        ///     Get's all Workspace for current User
        /// </summary>
        /// <remarks> Needs: nothing <br />  Returns: WorkspaceListResult </remarks>
        /// <returns></returns>
        /// <response code="200">Returns a WorkspaceListResult</response>

        [HttpGet(ApiMethod.GetallByUserId)]
        public async Task<IActionResult> GetallByUserId()
        {
            try
            {
                var userId = GetUserId();
                List<Workspace> workspaceList = await _unitOfWork.WorkspaceRepository.GetMultipleByAsync(x => x.UserId == userId);
                var list = _mapper.Map<List<WorkspaceDto>>(workspaceList);
                //return new JsonResult(list);
                return new JsonResult(new WorkspaceListResult { Workspaces = list, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new WorkspaceListResult { Error = ex.Message, Successful = false });
            }
        }
        
        /// <summary>
        /// GetOneById
        /// </summary>
        /// <param name="id"></param>
        /// <returns>WorkspaceResult</returns>
        [HttpGet(ApiMethod.GetOneById)]
        public async Task<IActionResult> GetOneById(int id)
        {
            try
            {

                //Int32.TryParse(stringId, out int id);
                if (!await CheckForPermissionByWorkspace(id, _unitOfWork))
                    return new JsonResult(new WorkspaceResult
                        { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                var item = await _unitOfWork.WorkspaceRepository.GetUniqueByAsync(x => x.Id == id);

                var result = new WorkspaceResult
                {
                    Successful = true,
                    Name = item.Name,
                    Id = item.Id
                };
                
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new WorkspaceResult { Error = ex.Message, Successful = false });
            }
        }
        
        /// <summary>
        /// GetFirstOrDefault
        /// </summary>
        /// <returns>WorkspaceResult</returns>
        [HttpGet(ApiMethod.GetFirstOrDefault)]
        public async Task<IActionResult> GetFirstOrDefault()
        {
            try
            {
                var userId = GetUserId();
                var item = await _unitOfWork.WorkspaceRepository.GetUniqueByAsync(x=>x.UserId == userId);
                if(item == null)
                {
                    item = new Workspace {UserId = userId, Name = "Default"};
                    await _unitOfWork.WorkspaceRepository.AddAsync(item);
                    await _unitOfWork.SaveAsync();
                }
                var result = new WorkspaceResult
                {
                    Successful = true,
                    Name       = item.Name,
                    Id         = item.Id 
                };
                
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new WorkspaceResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Creates a new Workspace
        /// </summary>
        /// <remarks>Needs: WorkspaceResourceNew <br /> Returns: WorkspaceResult</remarks>
        /// <param name="workspaceResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a WorkspaceResult</response>
        //[Authorize]
        [HttpPost(ApiMethod.Create)]
        public async Task<IActionResult> Create(WorkspaceResourceNew workspaceResource)
        {
            try
            {
                var userId = GetUserId();
                var workspace = new Workspace { Name = workspaceResource.Name, UserId = userId };
                await _unitOfWork.WorkspaceRepository.AddAsync(workspace);
                await _unitOfWork.SaveAsync();
                var newWorkspace = await _unitOfWork.WorkspaceRepository.GetUniqueByAsync(x => x.Name == workspaceResource.Name && x.UserId == userId);
                var workspaceResult = _mapper.Map<WorkspaceResult>(newWorkspace);
                workspaceResult.Successful = true;

                return new JsonResult(workspaceResult);
            }
            catch (Exception ex)
            {
                return new JsonResult(new WorkspaceResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Updates a Workspace
        /// </summary>
        /// <remarks>Needs: WorkspaceResource <br /> Returns: ResultModel</remarks>
        /// <param name="workspaceResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a ResultModel</response>
        //[Authorize]
        [HttpPost(ApiMethod.Update)]
        public async Task<IActionResult> Update(WorkspaceResource workspaceResource)
        {
            try
            {
                var workspace = await _unitOfWork.WorkspaceRepository
                    .GetUniqueByAsync(x => x.Id == workspaceResource.Id);

                if (!CheckForPermissionById(workspace.UserId))
                    return new JsonResult(new ResultModel
                    { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                workspace.Name = workspaceResource.Name;
                _unitOfWork.WorkspaceRepository.Update(workspace);
                await _unitOfWork.SaveAsync();
                return new JsonResult(new ResultModel { Error = null, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel { Error = ex.Message, Successful = false });
            }
        }


        /// <summary>
        ///     Deletes a Workspace
        /// </summary>
        /// <remarks>Needs: Id \
        /// Returns: ResultModel</remarks>
        /// <returns></returns>
        /// <response code="200">Returns a ResultModel</response>
        //[Authorize]
        [HttpGet(ApiMethod.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //Int32.TryParse(idString, out int id);
                if (id != 0)
                {
                    var workspace = await _unitOfWork.WorkspaceRepository
                        .GetUniqueByAsync(x => x.Id == id);

                    if (!CheckForPermissionById(workspace.UserId))
                        return new JsonResult(new ResultModel
                        { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                    _unitOfWork.WorkspaceRepository.Remove(workspace);
                    await _unitOfWork.SaveAsync();
                    return new JsonResult(new ResultModel { Error = null, Successful = true });
                }
                else
                {
                    return new JsonResult(new ResultModel { Error = "ID was 0.", Successful = false });
                }

            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel { Error = ex.Message, Successful = false });
            }
        }

    }
}