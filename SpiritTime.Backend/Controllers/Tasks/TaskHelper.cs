using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using SpiritTime.Core;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Helper;
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
        /// <param name="mapper"></param>
        public TaskHelper(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// CheckAndStopPreviousTask
        /// </summary>
        /// <param name="task"></param>
        /// <param name="workspaceId"></param>
        public async Task CheckAndStopPreviousTask(TaskDto task, int workspaceId)
        {
            if (task.EndDate == DateTime.MinValue)
            {
                var tasksNotEnded = await _unitOfWork.TaskRepository
                    .GetMultipleByAsync(x => x.EndDate == DateTime.MinValue &&
                                           x.Id != task.Id &&
                                           x.WorkspaceId == workspaceId);
                foreach (var taskNotEnded in tasksNotEnded)
                {
                    if (taskNotEnded != null)
                    {
                        taskNotEnded.EndDate = task.StartDate;
                        _unitOfWork.TaskRepository.Update(taskNotEnded);
                        await _unitOfWork.SaveAsync();
                    }
                }

            }
        }

        /// <summary>
        /// CheckAndStopAllTasks
        /// </summary>
        /// <param name="workspaceId"></param>
        public async Task CheckAndStopAllTasks(int workspaceId)
        {
            var tasksNotEnded = await _unitOfWork.TaskRepository
                    .GetMultipleByAsync(x => x.EndDate == DateTime.MinValue &&
                                             x.WorkspaceId == workspaceId);
            foreach (var taskNotEnded in tasksNotEnded.Where(taskNotEnded => taskNotEnded != null))
            {
                taskNotEnded.EndDate = DateTime.Now;
                _unitOfWork.TaskRepository.Update(taskNotEnded);
                await _unitOfWork.SaveAsync();
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
        /// <param name="removePrevious"></param>
        public async Task AddRangeOfTagsToTask(List<TagInfo> tagInfoList, int taskId, bool removePrevious = true)
        {
            var list = await GetAllCurrentlyLinkedTags(taskId);
            if (tagInfoList != null)
            {
                foreach (var item in tagInfoList)
                {
                    if(!CheckIfTagIsCurrentlyConnected(list, item))
                    {
                        await AddTagToTask(taskId, item.Id);
                    }
                }
            }

            if (removePrevious)
            {
                foreach (var item in list)
                {
                    await RemoveTagFromTask(taskId, item.Id);
                }
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
            var rules =  await _unitOfWork.TaskTagRuleRepository
                    .GetMultipleByAsync(x => x.Tag.WorkspaceId == workspaceId);
            return rules.OrderByDescending(x=>x.TriggerText).ToList();
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
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Contains(item.TriggerText))
                {
                    await AddTagToTask(taskId, item.TagId);
                    if (item.ReplaceTrigger)
                        text = text.Replace(item.TriggerText, "").Trim();
                }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<TaskDto> FindTagsByTrigger(TaskDto task)
        {
            var allTags = await _unitOfWork.TagRepository.GetMultipleByAsync(x => x.WorkspaceId == task.WorkspaceId);
            if (!string.IsNullOrEmpty(task.Name))
            {
                if(task.Name.Contains(SD.TagTrigger))
                {
                    var (taglist, name) = await GetTags(task, task.Name, allTags);
                    task.TagList.AddRange(taglist);
                    task.Name = name.Trim();
                }
            }

            if (!string.IsNullOrEmpty(task.Description))
            {
                if (!task.Description.Contains(SD.TagTrigger)) return task;
                
                var (taglist, description) = await GetTags(task, task.Description, allTags);
                task.TagList.AddRange(taglist);
                task.Description = description.Trim();
            }

            return task;
        }

        private async Task<(List<TagInfo>, string)> GetTags(TaskDto task, string text, List<Tag> allTags)
        {
            var names = GetTagsNames(text);
            var tagList = new List<Tag>();
            foreach (var name in names)
            {
                var tag = allTags.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
                if (tag != null)
                {
                    await AddTagToTask(task.Id, tag.Id);
                }
                else
                {
                    tag = new Tag {Name = name, WorkspaceId = task.WorkspaceId};
                    await _unitOfWork.TagRepository.AddAsync(tag);
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.TaskTagRepository.AddAsync(new TaskTag{TaskId = task.Id, TagId = tag.Id});
                    await _unitOfWork.SaveAsync();
                }
                tagList.Add(tag);
                text = text.Replace(SD.TagTrigger + name, "");
            }

            return (_mapper.Map<List<TagInfo>>(tagList), text);
        }


        private List<string> GetTagsNames(string text)
        {
            var elements = text.Split("##");

            return elements
                .Where(x=>x != elements.FirstOrDefault())
                .Select(element => element.Split(' ', 1).FirstOrDefault())
                .Select(x=> x?.Trim())
                .ToList();
        }

        /// <summary>
        /// Trim Task
        /// </summary>
        /// <param name="task"></param>
        public void TrimTask(Core.Entities.Tasks task)
        {
            if (task == null) return;
            task.Name        = !string.IsNullOrEmpty(task.Name) ? task.Name.Trim() : task.Name;
            task.Description = !string.IsNullOrEmpty(task.Description) ? task.Description.Trim() : task.Description;

        }
    }
}