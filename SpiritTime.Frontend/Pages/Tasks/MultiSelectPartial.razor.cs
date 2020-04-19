using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpiritTime.Core.Entities;
using SpiritTime.Frontend.Partials.ToastModal;
using SpiritTime.Frontend.Services.TaskServices;
using SpiritTime.Shared.Messages;
using SpiritTime.Shared.Models.TagModels;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Pages.Tasks
{
    public partial class MultiSelectPartial
    {
        [Inject] public IToastService ToastService { get; set; }
        [Inject] private ITaskService Service { get; set; }
        [Inject] private IMapper _mapper { get; set; }
        
        [Parameter] public TaskDto CurrentItem { get; set; }
        [Parameter] public  TaskDto TaskItem { get; set; }
        [Parameter] public  int CurrentSelectListId { get; set; }
        [Parameter] public  List<TagDto> TagList { get; set; }
        
        private bool ShowTagList { get; set; } = false;
        private  MultiSelectList MultiSelectList { get; set; }
        private bool ValueChanged { get; set; }
        
        private async Task AddTags(TaskDto item)
        {
            ShowTagList = false;
            if (ValueChanged)
            {
                ValueChanged = false;
                try
                {
                    var result = await Service.UpdateTags(item);
                    if (result.Successful)
                    {
                        ToastService.ShowSuccess(SuccessMsg.TagsForTaskEdited);
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

        }
        
        private void CheckboxChanged(ChangeEventArgs e, string key, TaskDto task)
        {
            ValueChanged = true;
            var selectItem = MultiSelectList.FirstOrDefault(i => i.Value == key);
            if (selectItem != null)
            {
                selectItem.Selected = (bool)e.Value;
                
                Int32.TryParse(key, out int id);
                if (id == 0) { ToastService.ShowError("Id could not be read"); return; }
                
                var tag = TagList.FirstOrDefault(x => x.Id == id);
                if (tag == null) { ToastService.ShowError("Tag could not be found"); return; }
                
                var tagInfo = _mapper.Map<TagInfo>(tag);
                if (selectItem.Selected)
                {
                    task.TagList.Add(tagInfo);
                }
                else
                {
                    task.TagList.Remove(task.TagList.FirstOrDefault(x=>x.Id == id));
                }
            }
        }
        
        private void OpenTagList(TaskDto item)
        {
            MultiSelectList = new MultiSelectList(TagList, nameof(CurrentItem.Id), nameof(CurrentItem.Name),item.TagList?.Select(x=>x.Id).ToList());

            CurrentSelectListId = item.Id;
            ValueChanged = false;
            ShowTagList = true;
            StateHasChanged();
            
        }
    }
}