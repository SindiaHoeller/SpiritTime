using System.Threading.Tasks;
using AutoMapper;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.TaskModels;

namespace SpiritTime.Backend.Infrastructure.Profiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Tasks, TaskDto>();
            CreateMap<TaskDto, Tasks>();
            CreateMap<Tasks, TaskNew>();
            CreateMap<TaskNew, Tasks>();
        }
    }
}