using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.TableServices;
using SpiritTime.Frontend.Services.TagServices;
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskModels;
using SpiritTime.Shared.Models.WorkspaceModels;
using Blazored.Typeahead;


namespace SpiritTime.Frontend.Pages.Tasks
{
    public partial class Overview
    {
        [Inject] private ITaskService Service { get; set; }
        [Inject] private ITagService TagService { get; set; }
        [Inject] private IOverlayModalService Modal { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [Parameter] public List<TagDto> TagList { get; set; }

        private bool ShowError { get; set; } = false;
        private string ErrorMessage { get; set; }
        private bool NoElements { get; set; }
        private TaskDto CurrentItem { get; set; }
        public TaskDto NewItem { get; set; }

        //private List<TaskDto> TaskDtoList { get; set; }
        private List<TaskDailyList> TaskDailyLists { get; set; }
        private int DayCount { get; set; } = 15;
        private string CurrentTime { get; set; } = "";
        private static Timer _timer;

        private bool ValueChanged { get; set; }

        public TagDto SelectedTag { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SelectedTag = new TagDto();
            NewItem = new TaskDto();
            TaskDailyLists = new List<TaskDailyList>();
            var result = await Service.GetTaskDailyList(DayCount);
            var result2 = await TagService.GetAllAsync();

            if (result.Item2.Successful)
            {
                if (result2.Successful)
                {
                    TagList = result2.ItemList;
                }
                else
                {
                    ToastService.ShowError(ErrorMsg.TagsCouldNotLoaded);
                }

                if (result.Item1?.Count > 0)
                {
                    TaskDailyLists = result.Item1.OrderByDescending(x => x.Date).ToList();
                    // Sets the timespan text for Dailylist AND the single tasks
                    TaskDailyLists.ForEach(x => x.TimeSpanText = Helper.GetTimSpanByTimeSpan(x.TimeSpan, false));
                    TaskDailyLists.ForEach(x =>
                        x.ItemList.ForEach(y =>
                            y.TimeSpanText = Helper.GetTimeSpanByDates(y.StartDate, y.EndDate, false)));

                    // Selects the currently running task, if available - selected by the Datetime not being specified
                    CurrentItem = TaskDailyLists
                        .Select(x => x.ItemList.FirstOrDefault(x => x.EndDate == DateTime.MinValue)).FirstOrDefault();
                    if (CurrentItem != null)
                    {
                        CurrentTime = Helper.GetTimeSpanByDates(CurrentItem.StartDate, DateTime.Now, true);
                        StartTimer();
                    }
                }
                else
                {
                    CurrentItem = new TaskDto();
                    NoElements = true;
                }
            }
            else
            {
                ErrorMessage = result.Item2.Error;
                ShowError = true;
            }
        }

        private async Task<IEnumerable<TagDto>> SearchTag(string searchText)
        {
            return await Task.FromResult(TagList.Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToList());
        }

        private void OnValueChanged()
        {
            ValueChanged = true;
        }


        private async Task Update(TaskDto item)
        {
            if (ValueChanged)
            {
                ValueChanged = false;
                try
                {
                    // Update the task
                    var result = await Service.Edit(item);
                    if (result.Successful)
                    {
                        // If not currently active item
                        if (item.EndDate != DateTime.MinValue)
                        {
                            // Update the task timespan text
                            item.TimeSpanText = Helper.UpdateTimeSpanText(item);
                            // Update the dailylist timespan text
                            Helper.UpdateTimeSpanTextForList(TaskDailyLists);
                        }

                        ToastService.ShowSuccess(SuccessMsg.SuccessedUpdate);
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
        }

        private async Task Start(TaskDto item)
        {
            try
            {
                StopTimer();
                var result = await Service.Add(item);
                if (result.Successful)
                {
                    await InvokeAsync(() =>
                    {
                        Helper.CheckAndAddCurrentItem(result.Item, CurrentItem, TaskDailyLists);

                        CurrentItem = result.Item;
                        NewItem = new TaskDto();

                        StartTimer();
                        ToastService.ShowSuccess(SuccessMsg.TimerStarted);
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
                        NewItem = new TaskDto();

                        ToastService.ShowSuccess(SuccessMsg.TimerStopped);
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

            // Set enddate for item and add it to the dailylist - create one if not available yet
        }

        private void StopTimer()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private void StartTimer()
        {
            if(_timer != null)
                StopTimer();
            SetTimer();
            // Console.WriteLine("Timer started");
        }


        private void SetTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimeEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void OnTimeEvent(Object source, ElapsedEventArgs e)
        {
            if (CurrentItem != null)
            {
                await InvokeAsync(() =>
                {
                    CurrentTime = Helper.GetTimeSpanByDates(CurrentItem.StartDate, DateTime.Now, true);
                    Console.WriteLine(CurrentTime);
                    StateHasChanged();
                });
            }

            else
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        private void SelectedTask()
        {
            ToastService.ShowInfo(SelectedTag?.Name);
            StartTimer();
        }

        private void OnFocusInStopTimer()
        {
            ToastService.ShowError("Stopped");
            StopTimer();
        }


        private void AddTag(TaskDto item)
        {
            var parameters = new OverlayModalParameters();
            parameters.Add(SD.ItemList, TagList);

            Modal.OnClose += EditResult;
            Modal.Show<TagSearch>(TextMsg.TagRuleEdit, parameters);
        }

        private void EditResult(OverlayModalResult modalResult)
        {
            if (!modalResult.Cancelled && modalResult.Data != null)
            {
                var item = (TagDto)modalResult.Data;

                if (item != null)
                {
                    Console.WriteLine(item.Name);
                }
            }
            Modal.OnClose -= EditResult;
        }
    }
}