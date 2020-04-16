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
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TaskModels;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Pages.Tasks
{
    public partial class Overview
    {
        [Inject] private ITaskService Service { get; set; }
        [Inject] private IOverlayModalService Modal { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        
        private bool ShowError { get; set; } = false;
        private string ErrorMsg { get; set; }
        private bool NoElements { get; set; }
        private TaskDto CurrentItem { get; set; }
        public TaskDto NewItem { get; set; }
        
        //private List<TaskDto> TaskDtoList { get; set; }
        private List<TaskDailyList> TaskDailyLists { get; set; }
        private int DayCount { get; set; } = 15;
        private string CurrentTime { get; set; } = "";
        private static Timer _timer;

        private bool ValueChanged { get; set; }
        protected override async Task OnInitializedAsync()
        {
            NewItem = new TaskDto();
            TaskDailyLists = new List<TaskDailyList>();
            var result = await Service.GetTaskDailyList(DayCount);
            
            if (result.Item2.Successful)
            {
                if (result.Item1?.Count > 0)
                {
                    TaskDailyLists = result.Item1.OrderByDescending(x=>x.Date).ToList();
                    // Sets the timespan text for Dailylist AND the single tasks
                    TaskDailyLists.ForEach(x=>x.TimeSpanText = Helper.GetTimSpanByTimeSpan(x.TimeSpan, false));
                    TaskDailyLists.ForEach(x=>x.ItemList.ForEach(y=>y.TimeSpanText = Helper.GetTimeSpanByDates(y.StartDate, y.EndDate, false)));
                    
                    // Selects the currently running task, if available - selected by the Datetime not being specified
                    CurrentItem  = TaskDailyLists.Select(x=>x.ItemList.FirstOrDefault(x=>x.EndDate == DateTime.MinValue)).FirstOrDefault();
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
                ErrorMsg = result.Item2.Error;
                ShowError = true;
            }
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
            SetTimer();
            // Console.WriteLine("Timer started");
        }

        
        private void SetTimer()
        {
            _timer = new Timer(1000);
            // Console.WriteLine("Timer initiated");
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
                    // Console.WriteLine(CurrentTime);
                    StateHasChanged();
                });
            }
                
            else
            {
                _timer.Stop();
                _timer.Dispose();
            }
                
        }


        // private void Update(TaskDto item)
        // {
        //     var parameters = new OverlayModalParameters();
        //     parameters.Add(SD.Item, item);
        //
        //     Modal.OnClose += EditResult;
        //     Modal.Show<Edit>(TextMsg.TaskEdit, parameters);
        // }
        // private void EditResult(OverlayModalResult modalResult)
        // {
        //     if(!modalResult.Cancelled && modalResult.Data != null)
        //     {
        //         var item = (TaskDto)modalResult.Data;
        //         
        //         if(item != null)
        //         {
        //             var itemOld = TaskDtoList.FirstOrDefault(x => x.Id == item.Id);
        //             TaskDtoList.Remove(itemOld);
        //             TaskDtoList.Add(item);
        //             StateHasChanged();
        //         }
        //     }
        //     Modal.OnClose -= EditResult;
        // }
    }
}