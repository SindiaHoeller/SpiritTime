using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Logging;
using SpiritTime.Backend.Controllers.Workspaces;
using SpiritTime.Backend.Infrastructure.Jwt;
using SpiritTime.Core;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskModels;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Backend.Controllers.Tasks
{
    /// <summary>
    /// TaskTagRuleController
    /// </summary>
    [ApiController]
    [Route(ControllerNames.Tasks)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskController : ControllerHelper
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<WorkspacesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TaskHelper Helper { get; set; }
        /// <summary>
        /// TaskTagRuleController
        /// </summary>
        /// <param name="jwtAuthentication"></param>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        public TaskController(JwtAuthentication jwtAuthentication,
            ILogger<WorkspacesController> logger,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork, IMapper mapper) : base(httpContextAccessor)
        {
            Helper = new TaskHelper(unitOfWork, mapper);
            _logger = logger;
            _jwtAuthentication = jwtAuthentication;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        /// <summary>
        ///     Get's all Tasks for Workplace Id
        /// </summary>
        /// <remarks> Needs: nothing <br />  Returns: TaskListResult </remarks>
        /// <returns></returns>
        /// <response code="200">Returns a TaskListResult</response>

        [HttpGet(ApiMethod.GetAllByWorkspace)]
        public async Task<IActionResult> GetAllByWorkspace(int id)
        {
            try
            {
                //Int32.TryParse(stringId, out int id);
                var all = await _unitOfWork.TaskRepository.GetMultipleByAsync(x=>x.WorkspaceId == id);
                
                var list = _mapper.Map<List<TaskDto>>(all);
                list = await Helper.GetAllTagsForTaskList(list);
                
                return new JsonResult(new TaskListResult { ItemList = list, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new TaskListResult { Error = ex.Message, Successful = false });
            }
        }
        
        /// <summary>
        ///     Get's one Task by its Id
        /// </summary>
        /// <remarks> Needs: Task Id <br />  Returns: TaskResult </remarks>
        /// <returns></returns>
        /// <response code="200">Returns a TaskResult</response>

        [HttpGet(ApiMethod.GetOneById)]
        public async Task<IActionResult> GetOneById(int id)
        {
            try
            {

                //Int32.TryParse(stringId, out int id);
                if (!await CheckForPermissionByTask(id, _unitOfWork))
                    return new JsonResult(new TaskResult
                        { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                var task = _unitOfWork.TaskRepository.GetUniqueByAsync(x => x.Id == id);
                var taskDto = _mapper.Map<TaskDto>(task);
                taskDto.TagList = await Helper.GetAllTagsForTask(id);
                var result = new TaskResult
                {
                    Successful = true,
                    Item = taskDto
                };
                
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new TaskResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Creates a new Task
        /// </summary>
        /// <remarks>Needs: TaskNew <br /> Returns: TaskResult</remarks>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a TaskResult</response>
        //[Authorize]
        [HttpPost(ApiMethod.Create)]
        public async Task<IActionResult> Create(TaskNew resource)
        {
            try
            {
                if (!await CheckForPermissionByWorkspace(resource.WorkspaceId, _unitOfWork))
                    return new JsonResult(new TagResult
                    { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });
                
                var item = _mapper.Map<Core.Entities.Tasks>(resource);
                await _unitOfWork.TaskRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                
                var resultItem = _mapper.Map<TaskDto>(item);
                resultItem = await Helper.AddTagsByRules(resource.WorkspaceId, resultItem);
                var result = new TaskResult
                {
                    Item = resultItem,
                    Successful = true
                };
                result.Item.TagList = await Helper.GetAllTagsForTask(item.Id);
                await Helper.CheckAndStopPreviousTask(resultItem);
                await Helper.AddRangeOfTagsToTask(resource.TagList, item.Id);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new TaskResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Updates a Task
        /// </summary>
        /// <remarks>Needs: TaskDto <br /> Returns: ResultModel</remarks>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a ResultModel</response>
        //[Authorize]
        [HttpPost(ApiMethod.Update)]
        public async Task<IActionResult> Update(TaskDto resource)
        {
            try
            {
                if (!await CheckForPermissionByWorkspace(resource.WorkspaceId, _unitOfWork))
                    return new JsonResult(new ResultModel
                    { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });
                var item = await _unitOfWork.TaskRepository
                    .GetUniqueByAsync(x => x.Id == resource.Id);

                var updated = _mapper.Map<Core.Entities.Tasks>(resource);
                updated.Id = item.Id;
                _unitOfWork.TaskRepository.Update(updated);
                await Helper.AddRangeOfTagsToTask(resource.TagList, item.Id);
                
                await _unitOfWork.SaveAsync();
                return new JsonResult(new ResultModel { Error = null, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel { Error = ex.Message, Successful = false });
            }
        }


        /// <summary>
        ///     Deletes a Task
        /// </summary>
        /// <remarks>Needs: Id \
        /// Returns: ResultModel</remarks>
        /// <param name="id"></param>
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
                    var item = await _unitOfWork.TaskRepository
                        .GetUniqueByAsync(x => x.Id == id);
                    if (!await CheckForPermissionByWorkspace(item.WorkspaceId, _unitOfWork))
                        return new JsonResult(new ResultModel
                            { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                    _unitOfWork.TaskRepository.Remove(item);
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