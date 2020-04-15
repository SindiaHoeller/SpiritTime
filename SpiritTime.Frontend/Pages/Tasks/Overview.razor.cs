using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using SpiritTime.Frontend.Partials.OverlayModalService;
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
        
        private bool ShowError { get; set; } = false;
        private string ErrorMsg { get; set; }
        private bool NoElements { get; set; }
        public TaskDto CurrentItem { get; set; }
        public TaskDto SelectedItem { get; set; }
        
        //private List<TaskDto> TaskDtoList { get; set; }
        private List<TaskDailyList> TaskDailyLists { get; set; }
        private int DayCount { get; set; } = 15;
        public string CurrentTime { get; set; } = "";

        [Parameter] public EventCallback<string> TimeChanged { get; set; }
        protected override async Task OnInitializedAsync()
        {
            TaskDailyLists = new List<TaskDailyList>();
            var result = await Service.GetTaskDailyList(DayCount);
            
            if (result.Item2.Successful)
            {
                if (result.Item1?.Count > 0)
                {
                    TaskDailyLists = result.Item1.OrderByDescending(x=>x.Date).ToList();
                    CurrentItem  = TaskDailyLists.Select(x=>x.ItemList.FirstOrDefault(x=>x.EndDate == DateTime.MinValue)).FirstOrDefault();
                    if (CurrentItem != null)
                    {
                        CurrentTime = GetTimeSpanByDates(CurrentItem.StartDate, DateTime.Now, true);
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

        private async Task AddAndStartTimer()
        {
            
        }
        void OnRowUpdating(SavedRowItem<TaskDto, Dictionary<string, object>> e) {
            //await TaskDtoList.Update(dataItem, newValue);
            var item = e.Item;
            var name = item.Name;
            Console.WriteLine("Dataitem: " + name);
        }

        private void Update(TaskDto item)
        {
            
        }

        private void Start(TaskDto item)
        {
            StartTimer();
        }
        private void Stop(TaskDto item)
        {
            aTimer.Stop();
            aTimer.Dispose();
        }

        private void StartTimer()
        {
            SetTimer();
            Console.WriteLine("Timer started");
        }

        private static Timer aTimer;
        private void SetTimer()
        {
            aTimer = new Timer(1000);
            Console.WriteLine("Timer initiated");
            aTimer.Elapsed += OnTimeEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private async void OnTimeEvent(Object source, ElapsedEventArgs e)
        {
            if (CurrentItem != null)
            {
                await InvokeAsync(() =>
                {
                    CurrentTime = GetTimeSpanByDates(CurrentItem.StartDate, DateTime.Now, true);
                    Console.WriteLine(CurrentTime);
                    StateHasChanged();
                });
                // await TimeChanged.InvokeAsync(CurrentTime);
                
            }
                
            else
            {
                aTimer.Stop();
                aTimer.Dispose();
            }
                
        }

        private string GetTimeSpanByDates(DateTime prev, DateTime after, bool includeSecs)
        {
            var span = after.Subtract(prev);
            return GetTimSpanByTimeSpan(span, includeSecs);
        }

        private string GetTimSpanByTimeSpan(TimeSpan span, bool includeSecs)
        {
            var timeSpanString = span.Days > 0 ? span.Days + " days - " : "";
            timeSpanString += span.Hours > 9 ? span.Hours + ":" : "0" + span.Hours + ":";
            timeSpanString += span.Minutes > 9 ? span.Minutes.ToString() : "0" + span.Minutes;
            if(includeSecs)
                timeSpanString += span.Seconds > 9 ? ":" + span.Seconds : ":0" + span.Seconds;
            return timeSpanString;
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