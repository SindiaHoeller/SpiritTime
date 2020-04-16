using System;
using System.Collections.Generic;
using System.Linq;
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

        public static string GetTimSpanByTimeSpan(TimeSpan span, bool includeSecs)
        {
            var timeSpanString = span.Days > 0 ? span.Days + " days - " : "";
            timeSpanString += span.Hours > 9 ? span.Hours + ":" : "0" + span.Hours + ":";
            timeSpanString += span.Minutes > 9 ? span.Minutes.ToString() : "0" + span.Minutes;
            if(includeSecs)
                timeSpanString += span.Seconds > 9 ? ":" + span.Seconds : ":0" + span.Seconds;
            return timeSpanString;
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
            if (currentItem == null) return;
            
            currentItem.EndDate = newItem.StartDate;
            AddCurrentItemToDailyList(currentItem, taskDailyLists );
        }

        public static void AddCurrentItemToDailyList(TaskDto currentItem, List<TaskDailyList> taskDailyLists)
        {
            var list = taskDailyLists.FirstOrDefault(x => x.Date.ToShortDateString() == currentItem.StartDate.ToShortDateString());
            if (list != null)
            {
                list.ItemList.Add(currentItem);
                UpdateTimeSpanTextForList(taskDailyLists);
            }
            else
            {
                var newDailyList = new TaskDailyList
                {
                    Date = currentItem.StartDate,
                    ItemList = new List<TaskDto> {currentItem}
                };
                newDailyList.TimeSpanText =  GetTimSpanByTimeSpan(newDailyList.TimeSpan, false);
                taskDailyLists.Add(newDailyList);
            }
        }
    }
}