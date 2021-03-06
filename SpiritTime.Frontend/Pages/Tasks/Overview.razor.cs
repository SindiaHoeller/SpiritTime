using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BlazorContextMenu;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpiritTime.Frontend.Infrastructure;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskModels;


namespace SpiritTime.Frontend.Pages.Tasks
{
    public partial class Overview : IDisposable
    {
        [Inject]    private ITaskService  Service      { get; set; }
        [Inject]    private ITagService   TagService   { get; set; }
        [Inject]    private IToastService ToastService { get; set; }
        [Inject]    private SelectState   SelectState  { get; set; }
        [Inject]    private IJSRuntime    JsRuntime    { get; set; }
        [Parameter] public  List<TagDto>  TagList      { get; set; }
        [Parameter] public  TagDto        SelectedTag  { get; set; }
        [Parameter] public  TaskDto       CurrentItem  { get; set; }
        [Inject] private IJSRuntime jsRuntime { get; set; }

        private        bool                ShowError          { get; set; } = false;
        private        string              ErrorMessage       { get; set; }
        private        bool                NoElements         { get; set; }
        private        bool                ShowFilterOptions  { get; set; }
        private        bool                HideTags           { get; set; }
        private        bool                HideBookingOptions { get; set; }
        private        TaskDto             NewItem            { get; set; }
        private        List<TaskDailyList> TaskDailyLists     { get; set; }
        private        int                 DayCount           { get; set; } = 15;
        private        string              CurrentTime        { get; set; } = "";
        private static Timer               _timer;
        private        bool                ValueChanged { get; set; }
        private        bool                IsDisabled   { get; set; }
        public DateTime Date { get; set; }
        private DateTime valueDateTime = DateTime.Now;

        //Needed for changing focus on datepicker
        private ElementReference focusHelper;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                InitItems();
                await GetDailyLists();
                await GetTagListResult();
                await GetFilter();
                SelectState.OnChange          += StateHasChanged;
                SelectState.OnSaveAndCloseAll += AddTags;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ToastService.ShowError(ex.Message);
            }
        }

        public void Dispose()
        {
            SelectState.OnChange          -= StateHasChanged;
            SelectState.OnSaveAndCloseAll -= AddTags;
        }

        private async Task GetFilter()
        {
            var hideTags = await Service.GetLocalStorageByKey(SD.HideTagsKey);
            if (hideTags == "true")
                HideTags = true;
            var hideBooking = await Service.GetLocalStorageByKey(SD.HideBookingOptionsKey);
            if (hideBooking == "true")
                HideBookingOptions = true;
        }


        #region Initial functions

        private async Task GetDailyLists()
        {
            var result = await Service.GetTaskDailyList(DayCount);
            if (result.Item2.Successful)
            {
                if (result.Item1?.Count > 0)
                {
                    TaskDailyLists = result.Item1.OrderByDescending(x => x.Date).ToList();
                    SetInitialTimespanText();
                    SelectCurrentlyRunningItem();
                }
                else
                {
                    NewItem    = new TaskDto();
                    NoElements = true;
                }
            }
            else
            {
                ToastService.ShowError(result.Item2.Error);
            }
        }

        private async Task GetTagListResult()
        {
            var result2 = await TagService.GetAllAsync();
            if (result2.Successful)
            {
                TagList = result2.ItemList;
            }
            else
            {
                ToastService.ShowError(ErrorMsg.TagsCouldNotLoaded);
            }
        }

        private void SetInitialTimespanText()
        {
            // Sets the timespan text for Dailylist AND the single tasks
            TaskDailyLists.ForEach(x => x.TimeSpanText = Helper.GetTimSpanByTimeSpan(x.TimeSpan, false));
            TaskDailyLists.ForEach(x =>
                x.ItemList.ForEach(y =>
                    y.TimeSpanText = Helper.GetTimeSpanByDates(y.StartDate, y.EndDate, false)));
        }

        private void SelectCurrentlyRunningItem()
        {
            // Selects the currently running task, if available - selected by the Datetime not being specified
            CurrentItem = TaskDailyLists
                .Select(x => x.ItemList.FirstOrDefault(dto => dto.EndDate == DateTime.MinValue)).FirstOrDefault(taskDto => taskDto != null);
            if (CurrentItem != null)
            {
                TaskDailyLists.ForEach(x => x.ItemList.Remove(CurrentItem));
                CurrentTime = Helper.GetTimeSpanByDates(CurrentItem.StartDate, DateTime.Now, true);
                StartTimer();
            }
        }

        private void InitItems()
        {
            SelectedTag    = new TagDto();
            NewItem        = new TaskDto();
            TaskDailyLists = new List<TaskDailyList>();
        }

        #endregion

        /// <summary>
        /// Tasks Update / Delete / Add
        /// </summary>

        #region Tasks Update & Delete & Add

        private void OnValueChanged()
        {
            ValueChanged = true;
        }

        private async Task OnValueChangedFocusOut()
        {
            ValueChanged = true;
            await focusHelper.Focus(JsRuntime);
        }
        

        private void ToActualDate(DateTime actual)
        {
            valueDateTime = actual;
            StateHasChanged();
        }

        private async void WriteBackStartDate(TaskDto task)
        {
            if (task.StartDate != valueDateTime)
            {
                task.StartDate = valueDateTime.Date.Add(task.StartDate.TimeOfDay);
                await UpdateStartDate(task);
            }
        }
        
        private async void WriteBackEndDate(TaskDto task)
        {
            if (task.EndDate != valueDateTime)
            {
                task.EndDate = valueDateTime.Date.Add(task.EndDate.TimeOfDay);
                await Update(task);
            }
        }


        private async Task UpdateCheckbox(TaskDto item)
        {
            if (!IsDisabled)
            {
                IsDisabled = true;
                StateHasChanged();
                item.IsBooked = !item.IsBooked;
                ValueChanged  = true;
                await Update(item);
            }
        }

        private async Task UpdateStartDate(TaskDto item)
        {
            if (ValueChanged)
            {
                var list = TaskDailyLists.FirstOrDefault(x => x.ItemList.Contains(item));
                if (item.StartDate.ToShortDateString() != list?.Date.ToShortDateString() && list != null)
                {
                    var listContainsDate = TaskDailyLists.FirstOrDefault(x => x.Date.ToShortDateString() == item.StartDate.ToShortDateString());
                    list.ItemList.Remove(item);
                    if (listContainsDate != null)
                    {
                        listContainsDate.ItemList.Add(item);
                        listContainsDate.ItemList = listContainsDate.ItemList.OrderByDescending(x => x.StartDate).ToList();
                    }
                    else
                    {
                        TaskDailyLists.Add(
                            new TaskDailyList
                            {
                                Date     = item.StartDate,
                                ItemList = new List<TaskDto> {item}
                            }
                        );
                    }

                    if (!list.ItemList.Any())
                    {
                        TaskDailyLists.Remove(list);
                    }
                }

                await Update(item);
            }
        }

        private async Task Update(TaskDto item)
        {
            if (ValueChanged)
            {
                ValueChanged = false;
                try
                {
                    GeneralHelper.TrimTask(item);
                    // Update the task
                    var result = await Service.Edit(item);
                    if (result.Successful)
                    {
                        // If not currently active item
                        if (item.EndDate != DateTime.MinValue)
                        {
                            // Update the task timespan text
                            item.TimeSpanText = Helper.UpdateTimeSpanText(item);

                            // Update basic task info
                            Helper.UpdateTaskInfo(item, result.Item);

                            // Add possibly missing tags to list
                            Helper.AddMissingTags(TagList, item.TagList, await Service.GetLocalStorageByKey(SD.CurrentWorkspace));


                            // Update the dailylist timespan text
                            Helper.UpdateTimeSpanTextForList(TaskDailyLists);
                        }
                        StateHasChanged();

                        // ToastService.ShowSuccess(SuccessMsg.SuccessedUpdate);
                        IsDisabled = false;
                    }
                    else
                    {
                        ToastService.ShowError(result.Error);
                        IsDisabled = false;
                    }
                }
                catch (Exception exception)
                {
                    ToastService.ShowError(exception.Message);
                    IsDisabled = false;
                }
            }
        }

        private async Task Start(TaskDto item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Name) && string.IsNullOrEmpty(item.Description))
                {
                    ToastService.ShowError(ErrorMsg.TaskEmpty);
                    return;
                }

                StopTimer();
                GeneralHelper.TrimTask(item);
                var result = await Service.Add(item);
                if (result.Successful)
                {
                    await InvokeAsync(() =>
                    {
                        Helper.CheckAndAddCurrentItem(result.Item, CurrentItem, TaskDailyLists);

                        CurrentItem = result.Item;
                        //Helper.AddCurrentItemToDailyList(CurrentItem, TaskDailyLists);
                        NewItem = new TaskDto();

                        StartTimer();
                        // ToastService.ShowSuccess(SuccessMsg.TimerStarted);
                        StateHasChanged();
                    });
                }
                else
                {
                    ToastService.ShowError(result.Error);
                }
            }
            catch (Exception exception)
            {
                ToastService.ShowError(exception.Message);
            }
        }


        private async Task Stop()
        {
            try
            {
                if (CurrentItem == null) return;

                StopTimer();
                CurrentItem.EndDate = DateTime.Now;
                var result = await Service.Edit(CurrentItem);
                if (result.Successful)
                {
                    await InvokeAsync(() =>
                    {
                        Helper.AddCurrentItemToDailyList(CurrentItem, TaskDailyLists);

                        CurrentItem = null;
                        NewItem     = new TaskDto();

                        // ToastService.ShowSuccess(SuccessMsg.TimerStopped);
                        StateHasChanged();
                    });
                }
                else
                {
                    ToastService.ShowError(result.Error);
                }
            }
            catch (Exception exception)
            {
                ToastService.ShowError(exception.Message);
            }
        }

        #endregion

        /// <summary>
        /// Timer functions
        /// </summary>

        #region Timer funktions

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        private void StartTimer()
        {
            if (_timer != null)
                StopTimer();
            SetTimer();
        }


        private void SetTimer()
        {
            _timer           =  new Timer(1000);
            _timer.Elapsed   += OnTimeEvent;
            _timer.AutoReset =  true;
            _timer.Enabled   =  true;
        }

        private async void OnTimeEvent(Object source, ElapsedEventArgs e)
        {
            if (CurrentItem != null)
            {
                await InvokeAsync(() =>
                {
                    CurrentTime = Helper.GetTimeSpanByDates(CurrentItem.StartDate, DateTime.Now, true);
                    StateHasChanged();
                });
            }
            else
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        #endregion


        private async void AddTags()
        {
            try
            {
                var item = CurrentItem?.Id == SelectState.CurrentSelectListId
                    ? CurrentItem
                    : Helper.GetTaskById(TaskDailyLists, SelectState.CurrentSelectListId);

                if (item == null || !SelectState.ValueChanged) return;

                SelectState.ValueChanged = false;
                var result = await Service.UpdateTags(item);
                if (result.Successful)
                {
                    // ToastService.ShowSuccess(SuccessMsg.TaskEdited + item.Name);
                    foreach (var tag in item.TagList)
                    {
                        Console.WriteLine(tag.Name);
                    }
                }
                else
                {
                    ToastService.ShowError(result.Error);
                }
            }
            catch (Exception exception)
            {
                ToastService.ShowError(exception.Message);
            }
        }

        private async void DeleteClick(ItemClickEventArgs e)
        {
            try
            {
                var item = e.Data as TaskDto;
                if (item == null) return;
                var result = await Service.Delete(item.Id);
                if (result.Successful)
                {
                    ToastService.ShowSuccess(SuccessMsg.TaskDeleted + item.Name);
                    Helper.RemoveItemFromDailyList(item, TaskDailyLists);
                    StateHasChanged();
                }
                else
                {
                    ToastService.ShowError(result.Error);
                }
            }
            catch (Exception exception)
            {
                ToastService.ShowError(exception.Message);
            }
            
            
        }
        private async void ToggleBooked(ItemClickEventArgs e)
        {
            var item = e.Data as TaskDto;
            await UpdateCheckbox(item);
        }


        //
        // private void ToDateTimeNow()
        // {
        //     testDateTime = DateTime.Now;
        //     StateHasChanged();
        // }
        //
        // private void TestChanged()
        // {
        //     var test = testDateTime;
        //     StateHasChanged();
        // }
        

        #region Filter
        private void ToggleFilterOptions()
        {
            ShowFilterOptions = !ShowFilterOptions;
            StateHasChanged();
        }

        private async Task TriggerHideTags()
        {
            StateHasChanged();
            await Service.SetLocalStorageByKey(SD.HideTagsKey, !HideTags ? "true" : "false");
        }

        private async Task TriggerHideBookingOptions()
        {
            StateHasChanged();
            await Service.SetLocalStorageByKey(SD.HideBookingOptionsKey, !HideBookingOptions ? "true" : "false");
        }

        #endregion
    }
}