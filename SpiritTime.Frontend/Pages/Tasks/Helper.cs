using System;
using System.Collections.Generic;
using System.Linq;
using SpiritTime.Frontend.Infrastructure;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Pages.Tasks
{
    public static class Helper
    {
        public static string GetTimeSpanByDates(DateTime prev, DateTime after, bool includeSecs)
        {
            var span = after.Subtract(prev);
            return GetTimSpanByTimeSpan(span, includeSecs);
        }

        public static void UpdateTaskInfo(TaskDto task, TaskDto newTask)
        {
            task.Name = newTask.Name;
            task.Description = newTask.Description;
            task.TagList = newTask.TagList;
        }

        public static void AddMissingTags(List<TagDto> tagList, List<TagInfo> newTagList, string workspaceId)
        {
            var workspace = Int32.TryParse(workspaceId, out int id) ? id : 0;
            foreach (var tag in newTagList)
            {
                if (tagList.FirstOrDefault(x => x.Id == tag.Id) == null)
                {
                    tagList.Add(new TagDto{Name = tag.Name, Id = tag.Id, WorkspaceId = workspace});
                }
            }
        }

        public static string GetTimSpanByTimeSpan(TimeSpan span, bool includeSecs)
        {
            var timeSpanString = "";
            if (span.CompareTo(TimeSpan.Zero) < 0)
            {
                span = span.Duration();
                timeSpanString += "MINUS ";
            }
            //timeSpanString += span.Days > 0 ? span.Days + "days " : "";
            var hours = span.Hours + span.Days * 24;
            timeSpanString += hours > 9 ? hours + ":" : "0" + hours + ":";
            timeSpanString += span.Minutes > 9 ? span.Minutes.ToString() : "0" + span.Minutes;
            if(includeSecs)
                timeSpanString += span.Seconds > 9 ? ":" + span.Seconds : ":0" + span.Seconds;
            return timeSpanString;
        }

        public static TaskDto GetTaskById(List<TaskDailyList> taskDailyLists, int id)
        {
            return taskDailyLists.Select(dailyList => dailyList.ItemList.FirstOrDefault(x => x.Id == id)).FirstOrDefault(taskDto => taskDto != null);
        }

        public static void UpdateTimeSpanTextForList(List<TaskDailyList> list)
        {
            
            list.ForEach(x=>x.TimeSpanText = GetTimSpanByTimeSpan(x.TimeSpan, false));
        }

        public static string UpdateTimeSpanText(TaskDto item)
        {
            return GetTimeSpanByDates(item.StartDate, item.EndDate, false);
        }
        
        public static void CheckAndAddCurrentItem(TaskDto newItem, TaskDto currentItem, List<TaskDailyList> taskDailyLists)
        {
            if (currentItem != null)
            {
                currentItem.EndDate = newItem.StartDate;
                AddCurrentItemToDailyList(currentItem, taskDailyLists);
            }
        }

        public static void AddCurrentItemToDailyList(TaskDto item, List<TaskDailyList> taskDailyLists)
        {
            var list = taskDailyLists.FirstOrDefault(x => x.Date.ToShortDateString() == item.StartDate.ToShortDateString());
            if (list != null)
            {
                item.TimeSpanText = GetTimeSpanByDates(item.StartDate, item.EndDate, false);
                list.ItemList.Insert(0, item);
                UpdateTimeSpanTextForList(taskDailyLists);
            }
            else
            {
                var newDailyList = new TaskDailyList
                {
                    Date = item.StartDate,
                    ItemList = new List<TaskDto> {item}
                };
                newDailyList.TimeSpanText =  GetTimSpanByTimeSpan(newDailyList.TimeSpan, false);
                taskDailyLists.Add(newDailyList);
            }
        }

        public static void RemoveItemFromDailyList(TaskDto item, List<TaskDailyList> taskDailyLists)
        {
            taskDailyLists.ForEach(x => x.ItemList.Remove(item));
        }
    }
}