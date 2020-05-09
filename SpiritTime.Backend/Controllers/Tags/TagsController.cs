using System;
using System.Collections.Generic;
using System.Linq;
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
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Backend.Controllers.Tags
{
    /// <summary>
    /// TagController
    /// </summary>
    [ApiController]
    [Route(ControllerNames.Tags)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : ControllerHelper
    {
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<WorkspacesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        /// <summary>
        /// TagController
        /// </summary>
        /// <param name="jwtAuthentication"></param>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        public TagsController(JwtAuthentication jwtAuthentication,
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
        ///     Get's all Tags
        /// </summary>
        /// <remarks> Needs: nothing <br />  Returns: TagListResult </remarks>
        /// <returns></returns>
        /// <response code="200">Returns a TagListResult</response>

        [HttpGet(ApiMethod.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = GetUserId();
                var all = await _unitOfWork.TagRepository.GetMultipleIncludeAsync(x=>x.Workspace.UserId == userId,x=>x.Workspace);
                var list = _mapper.Map<List<TagDto>>(all);
                
                return new JsonResult(new TagListResult { ItemList = list, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new TagListResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Creates a new Tag in the Workspace
        /// </summary>
        /// <remarks>Needs: TagResourceNew <br /> Returns: TagResult</remarks>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a TagResult</response>
        //[Authorize]
        [HttpPost(ApiMethod.Create)]
        public async Task<IActionResult> Create(TagResourceNew resource)
        {
            try
            {
                if (!await CheckForPermissionByWorkspace(resource.WorkspaceId, _unitOfWork))
                    return new JsonResult(new TagResult
                    { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });

                var item = new Tag { Name = resource.Name, ColorCode = resource.ColorCode, WorkspaceId = resource.WorkspaceId };
                await _unitOfWork.TagRepository.AddAsync(item);
                await _unitOfWork.SaveAsync();
                var newItem = await _unitOfWork.TagRepository
                    .GetUniqueByIncludeAsync(x => x.Name == resource.Name 
                                           && x.WorkspaceId == resource.WorkspaceId,
                        x=>x.Workspace);
                var result = new TagResult
                {
                    Item = _mapper.Map<TagDto>(newItem),
                    Successful = true
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new TagResult { Error = ex.Message, Successful = false });
            }
        }

        /// <summary>
        ///     Updates a Tag
        /// </summary>
        /// <remarks>Needs: TagResource <br /> Returns: ResultModel</remarks>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a ResultModel</response>
        //[Authorize]
        [HttpPost(ApiMethod.Update)]
        public async Task<IActionResult> Update(TagResource resource)
        {
            try
            {
                if (!await CheckForPermissionByWorkspace(resource.WorkspaceId, _unitOfWork))
                    return new JsonResult(new ResultModel
                    { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });
                var item = await _unitOfWork.TagRepository
                    .GetUniqueByAsync(x => x.Id == resource.Id);

                item.Name = resource.Name;
                item.WorkspaceId = resource.WorkspaceId;
                item.ColorCode = resource.ColorCode;
                _unitOfWork.TagRepository.Update(item);
                await _unitOfWork.SaveAsync();
                return new JsonResult(new ResultModel { Error = null, Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResultModel { Error = ex.Message, Successful = false });
            }
        }


        /// <summary>
        ///     Deletes a Tag
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
                    var item = await _unitOfWork.TagRepository
                        .GetUniqueByAsync(x => x.Id == id);
                    if (!await CheckForPermissionByWorkspace(item.WorkspaceId, _unitOfWork))
                        return new JsonResult(new ResultModel
                            { Error = ErrorMsg.NotAuthorizedForAction, Successful = false });


                    _unitOfWork.TagRepository.Remove(item);
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
