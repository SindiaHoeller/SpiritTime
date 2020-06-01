using System.Threading.Tasks;
using AutoMapper;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Backend.Infrastructure.Profiles
{
    /// <summary>
    /// TaskProfile
    /// </summary>
    public class TaskProfile : Profile
    {
        /// <summary>
        /// TaskProfile
        /// </summary>
        public TaskProfile()
        {
            CreateMap<Tasks, TaskDto>();
            CreateMap<TaskDto, Tasks>();
            CreateMap<Tasks, TaskNew>();
            CreateMap<TaskNew, Tasks>();
        }
    }
}