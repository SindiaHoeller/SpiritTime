using AutoMapper;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Frontend.Infrastructure.Profiles
{
    public class TaskTagRuleProfile : Profile
    {
        public TaskTagRuleProfile()
        {
            //CreateMap<TaskTagRuleNew, TaskTagRuleDto>();
            CreateMap<TaskTagRuleDto, TaskTagRuleNew>();
            CreateMap<TaskTagRuleResult, TaskTagRuleDto>();
            
        }
    }
}