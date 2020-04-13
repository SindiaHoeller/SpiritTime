using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpiritTime.Backend.Controllers.Tasks;
using SpiritTime.Backend.Controllers.Workspaces;
using SpiritTime.Backend.Infrastructure.Jwt;
using SpiritTime.Core;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models;
using SpiritTime.Shared.Models.TaskModels;
using SpiritTime.Shared.Models.TaskTagModels;

namespace SpiritTime.Backend.Controllers.TaskTags
{
    /// <summary>
    /// TaskTagController
    /// </summary>
    [ApiController]
    [Route(ControllerNames.TaskTag)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskTagController : ControllerHelper
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<WorkspacesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private TaskHelper Helper { get; set; }
        /// <summary>
        /// TaskTagRuleController
        /// </summary>
        /// <param name="jwtAuthentication"></param>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        public TaskTagController(JwtAuthentication jwtAuthentication,
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
        /// Add Tags to Tasks
        /// </summary>
        /// <param name="tasktag"></param>
        /// <returns></returns>
        [HttpPost(ApiMethod.AddTag)]
        public async Task<IActionResult> AddTag(TaskTagDto tasktag)
        {
            try
            {
                if (tasktag.TagId != 0 && tasktag.TaskId != 0)
                {
                    if (!await CheckForPermissionByTag(tasktag.TagId, _unitOfWork))
                        return new JsonResult(new ResultModel
                            { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                    var item = await _unitOfWork.TaskTagRepository.GetUniqueByAsync(x =>
                        x.TagId == tasktag.TagId && x.TaskId == tasktag.TaskId);
                    if (item == null)
                    {
                        item = new TaskTag
                        {
                            TagId = tasktag.TagId,
                            TaskId = tasktag.TaskId
                        };
                        await _unitOfWork.TaskTagRepository.AddAsync(item);
                        await _unitOfWork.SaveAsync();
                    }
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
        
        /// <summary>
        /// Remove Tags to Tasks
        /// </summary>
        /// <param name="tasktag"></param>
        /// <returns></returns>
        [HttpPost(ApiMethod.RemoveTag)]
        public async Task<IActionResult> RemoveTag(TaskTagDto tasktag)
        {
            try
            {
                if (tasktag.TagId != 0 && tasktag.TaskId != 0)
                {
                    if (!await CheckForPermissionByTag(tasktag.TagId, _unitOfWork))
                        return new JsonResult(new ResultModel
                            { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                    var item = await _unitOfWork.TaskTagRepository.GetUniqueByAsync(x =>
                        x.TagId == tasktag.TagId && x.TaskId == tasktag.TaskId);
                    if (item != null && item?.TagId <= 0)
                    {
                        _unitOfWork.TaskTagRepository.Remove(item);
                        await _unitOfWork.SaveAsync();
                    }
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
        
        /// <summary>
        /// Remove Tags to Tasks
        /// </summary>
        /// <param name="tasktag"></param>
        /// <returns></returns>
        [HttpPost(ApiMethod.CompareTags)]
        public async Task<IActionResult> CompareTags(TaskDto task)
        {
            try
            {
                if (task.Id != 0)
                {
                    if (!await CheckForPermissionByWorkspace(task.WorkspaceId, _unitOfWork))
                        return new JsonResult(new ResultModel
                            { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                    await Helper.AddRangeOfTagsToTask(task.TagList, task.Id);
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