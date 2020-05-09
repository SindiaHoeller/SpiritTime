using System;
using System.Collections.Generic;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Frontend.Infrastructure
{
    public class TaskComparer : IEqualityComparer<TaskDto>
    {
        public bool Equals(TaskDto x, TaskDto y)
        {
            return x?.Id == y?.Id;
        }

        public int GetHashCode(TaskDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}