using System;
using System.Collections.Generic;
using System.Linq;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Pages.Tasks
{
    public class SelectState
    {
        public SelectState()
        {
            ShowTagLists = new List<bool>();
        }

        public bool ValueChanged { get; set; }
        public List<bool> ShowTagLists { get; set; }
        public int CurrentSelectListId { get; set; }

        public event Action OnChange;
        public event Action OnSaveAndCloseAll;
        public void CloseAll()
        {
            if(CurrentSelectListId != 0)
                OnSaveAndCloseAll?.Invoke();
            for (var i = 0; i < ShowTagLists.Count; i++)
            {
                if (ShowTagLists[i])
                {
                    ShowTagLists[i] = false;
                }
            }

            CurrentSelectListId = 0;
            //ShowTagLists.ForEach(false);
            NotifyStateChanged();
        }

        public void CloseOne(int id)
        {
            if(ValueChanged)
                OnSaveAndCloseAll?.Invoke();
            ShowTagLists[id] = false;
            CurrentSelectListId = 0;
            NotifyStateChanged();
        }

        // public void AddItem(bool propChanged, TaskDto task, bool showtaglist = false)
        // {
        //     PropertiesChanged.Add(propChanged);
        //     TaskList.Add(task);
        //     ShowTagLists.Add(showtaglist);
        // }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}