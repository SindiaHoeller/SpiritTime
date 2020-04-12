using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Backend.Controllers.TaskTagRules
{
    /// <summary>
    /// TaskTagRuleController
    /// </summary>
    [ApiController]
    [Route(ControllerNames.TaskTagRules)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskTagRuleController : ControllerHelper
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<WorkspacesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        /// <summary>
        /// TaskTagRuleController
        /// </summary>
        /// <param name="jwtAuthentication"></param>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        public TaskTagRuleController(JwtAuthentication jwtAuthentication,
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
        ///     Get's all TaskTagRules
        /// </summary>
        /// <remarks> Needs: nothing <br />  Returns: TagListResult </remarks>
        /// <returns></returns>
        /// <response code="200">Returns a TagListResult</response>

        [HttpGet(ApiMethod.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var all = await _unitOfWork.TaskTagRuleRepository.GetAllIncludeAsync(x=>x.Tag);
                var list = _mapper.Map<List<TaskTagRuleDto>>(all);
                
                return new JsonResult(new TaskTagRuleListResult { ItemList = list, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new TaskTagRuleListResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Creates a new TaskTagResult
        /// </summary>
        /// <remarks>Needs: TaskTagRuleNew <br /> Returns: TaskTagRuleResult</remarks>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a TaskTagRuleResult</response>
        //[Authorize]
        [HttpPost(ApiMethod.Create)]
        public async Task<IActionResult> Create(TaskTagRuleNew resource)
        {
            try
            {
                if (!await CheckForPermissionByTag(resource.TagId, _unitOfWork))
                    return new JsonResult(new TagResult
                    { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                var item = _mapper.Map<Core.Entities.TaskTagRules>(resource);
                await _unitOfWork.TaskTagRuleRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                var newItem = await _unitOfWork.TaskTagRuleRepository
                    .GetUniqueByIncludeAsync(x => x.TagId == resource.TagId 
                                           && x.TriggerText == resource.TriggerText,
                        x=>x.Tag);
                var itemResult = _mapper.Map<TaskTagRuleDto>(newItem);
                var result = new TaskTagRuleResult
                {
                    Item = itemResult,
                    Successful = true
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new TaskTagRuleResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Updates a TaskTagRule
        /// </summary>
        /// <remarks>Needs: TaskTagRuleDto <br /> Returns: ResultModel</remarks>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a ResultModel</response>
        //[Authorize]
        [HttpPost(ApiMethod.Update)]
        public async Task<IActionResult> Update(TaskTagRuleDto resource)
        {
            try
            {
                if (!await CheckForPermissionByTag(resource.TagId, _unitOfWork))
                    return new JsonResult(new ResultModel
                    { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });
                var item = await _unitOfWork.TaskTagRuleRepository
                    .GetUniqueByAsync(x => x.Id == resource.Id);

                item.TagId = resource.TagId;
                item.ReplaceTrigger = resource.ReplaceTrigger;
                item.TriggerDescription = resource.TriggerDescription;
                item.TriggerName = resource.TriggerName;
                item.TriggerText = resource.TriggerText;
                _unitOfWork.TaskTagRuleRepository.Update(item);
                await _unitOfWork.SaveAsync();
                return new JsonResult(new ResultModel { Error = null, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel { Error = ex.Message, Successful = false });
            }
        }


        /// <summary>
        ///     Deletes a TaskTagRule
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
                    var item = await _unitOfWork.TaskTagRuleRepository
                        .GetUniqueByAsync(x => x.Id == id);
                    if (!await CheckForPermissionByTag(item.TagId, _unitOfWork))
                        return new JsonResult(new ResultModel
                            { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });


                    _unitOfWork.TaskTagRuleRepository.Remove(item);
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