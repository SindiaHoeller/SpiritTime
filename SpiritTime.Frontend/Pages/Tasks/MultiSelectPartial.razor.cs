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
        [Parameter] public  List<TagDto> TagList { get; set; }
        [Parameter] public SelectState SelectState { get; set; }
        // [Parameter] public bool ValueChanged { get; set; }
        // public event Action<TaskDto> OnClose;
        
        private int Id { get; set; }
        private  MultiSelectList MultiSelectList { get; set; }
        

        protected override async Task OnInitializedAsync()
        {
            SelectState.ShowTagLists.Add(false);
            Id = SelectState.ShowTagLists.Count -1;
            //ShowTagList = false;
        }

        private void CheckboxChanged(ChangeEventArgs e, string key)
        {
            SelectState.ValueChanged = true;
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
                    TaskItem.TagList.Add(tagInfo);
                }
                else
                {
                    TaskItem.TagList.Remove(TaskItem.TagList.FirstOrDefault(x=>x.Id == id));
                }
            }
        }

        private async Task CloseTagList()
        {
            SelectState.CloseOne(Id);
        }
        
        private void OpenTagList(TaskDto item)
        {
            if (SelectState.ShowTagLists[Id])
            {
                SelectState.CloseOne(Id);
            }
            else
            {
                SelectState.CloseAll();
                MultiSelectList = new MultiSelectList(TagList, nameof(CurrentItem.Id), nameof(CurrentItem.Name), item.TagList?.Select(x=>x.Id).ToList());
            
                SelectState.CurrentSelectListId          = item.Id;
                SelectState.ValueChanged     = false;
                SelectState.ShowTagLists[Id] = true;
                StateHasChanged();
            }
        }
    }
}