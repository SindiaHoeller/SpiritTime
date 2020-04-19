using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using SpiritTime.Core;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Backend.Controllers.Tasks
{
    /// <summary>
    /// Helps for all kind of special "Task" Tasks
    /// </summary>
    public class TaskHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public TaskHelper(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// CheckAndStopPreviousTask
        /// </summary>
        /// <param name="task"></param>
        public async Task CheckAndStopPreviousTask(TaskDto task)
        {
            if (task.EndDate == DateTime.MinValue)
            {
                var taskNotEnded = await _unitOfWork.TaskRepository
                    .GetUniqueByAsync(x => x.EndDate == DateTime.MinValue &&
                                           x.Id != task.Id);
                if (taskNotEnded != null)
                {
                    taskNotEnded.EndDate = task.StartDate;
                    _unitOfWork.TaskRepository.Update(taskNotEnded);
                    await _unitOfWork.SaveAsync();
                }
            }
        }
        
        /// <summary>
        /// Adds Tags to Tasks
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="tagId"></param>
        private async Task AddTagToTask(int taskId, int tagId)
        {
            try
            {
                var exists = await _unitOfWork.TaskTagRepository.ExistAsync(x => x.TaskId == taskId && x.TagId == tagId);
                if (!exists)
                {
                    await _unitOfWork.TaskTagRepository.AddAsync(new TaskTag {TaskId = taskId, TagId = tagId});
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        /// <summary>
        /// Removes Tags from Tasks
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="tagId"></param>
        private async Task RemoveTagFromTask(int taskId, int tagId)
        {
            try
            {
                var tasktag = await _unitOfWork.TaskTagRepository
                    .GetUniqueByAsync(x => x.TagId == tagId && x.TaskId == taskId);
                if (tasktag != null)
                {
                    _unitOfWork.TaskTagRepository.Remove(tasktag);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

        }

        /// <summary>
        /// Gets all currently linked Tags
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<List<TagInfo>> GetAllCurrentlyLinkedTags(int taskId)
        {
            var list =  await _unitOfWork.TaskTagRepository
                .SelectMulitpleAsync(x => x.TaskId == taskId, 
                    x => x.Tag);
            return _mapper.Map<List<TagInfo>>(list);
        }

        /// <summary>
        /// AddRangeOfTagsToTask
        /// </summary>
        /// <param name="tagInfoList"></param>
        /// <param name="taskId"></param>
        public async Task AddRangeOfTagsToTask(List<TagInfo> tagInfoList, int taskId)
        {
            var list = await GetAllCurrentlyLinkedTags(taskId);
            foreach (var item in tagInfoList)
            {
                if(!CheckIfTagIsCurrentlyConnected(list, item))
                {
                    await AddTagToTask(taskId, item.Id);
                }
            }

            foreach (var item in list)
            {
                await RemoveTagFromTask(taskId, item.Id);
            }
        }

        private bool CheckIfTagIsCurrentlyConnected(List<TagInfo> tagDtos, TagInfo info)
        {
            var itemInList = tagDtos.FirstOrDefault(x => x.Id == info.Id);
            
            if (itemInList == null) return false;
            
            tagDtos.Remove(itemInList);
            return true;

        }

        /// <summary>
        /// GetAllRulesForWorkspace
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <returns></returns>
        private async Task<List<Core.Entities.TaskTagRules>> GetAllRulesForWorkspace(int workspaceId)
        {
            return await _unitOfWork.TaskTagRuleRepository
                    .GetMultipleByAsync(x => x.Tag.WorkspaceId == workspaceId);
        }

        /// <summary>
        /// AddTagsByRules
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<TaskDto> AddTagsByRules(int workspaceId, TaskDto task)
        {
            var rules = await GetAllRulesForWorkspace(workspaceId);
            foreach (var item in rules)
            {
                if (item.TriggerName)
                {
                    task.Name = await ExecuteTrigger(item, task.Id, task.Name);
                }

                if (item.TriggerDescription)
                {
                    task.Description = await ExecuteTrigger(item, task.Id, task.Description);
                }
            }

            task.TagList = await GetAllCurrentlyLinkedTags(task.Id);
            return task;
        }

        /// <summary>
        /// ExecuteTrigger
        /// </summary>
        /// <param name="item"></param>
        /// <param name="taskId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private async Task<string> ExecuteTrigger(Core.Entities.TaskTagRules item, int taskId, string text)
        {
            if (text.Contains(item.TriggerText))
            {
                await AddTagToTask(taskId, item.TagId);
                if (item.ReplaceTrigger)
                    text = text.Replace(item.TriggerText, "");
            }

            return text;
        }

        /// <summary>
        /// GetAllTagsForTask
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<List<TagInfo>> GetAllTagsForTask(int taskId)
        {
            var list = await _unitOfWork.TaskTagRepository.SelectMulitpleAsync(x => x.TaskId == taskId,
                x => x.Tag);
            return _mapper.Map<List<TagInfo>>(list);
        }
        
        /// <summary>
        /// GetAllTagsForTaskList
        /// </summary>
        /// <param name="taskDtos"></param>
        /// <returns></returns>
        public async Task<List<TaskDto>> GetAllTagsForTaskList(List<TaskDto> taskDtos)
        {
            foreach (var item in taskDtos)
            {
                item.TagList = await GetAllTagsForTask(item.Id);
            }

            return taskDtos;
        }

    }
}