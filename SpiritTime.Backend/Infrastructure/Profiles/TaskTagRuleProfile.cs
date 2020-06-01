using AutoMapper;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.TaskTagRuleModels;

namespace SpiritTime.Backend.Infrastructure.Profiles
{
    /// <summary>
    /// TaskTagRuleProfile
    /// </summary>
    public class TaskTagRuleProfile : Profile
    {
        /// <summary>
        /// TaskTagRuleProfile
        /// </summary>
        public TaskTagRuleProfile()
        {
            CreateMap<TaskTagRules, TaskTagRuleDto>();
            CreateMap<TaskTagRules, TaskTagRuleNew>();
            CreateMap<TaskTagRuleNew, TaskTagRules>();
            CreateMap<TaskTagRuleDto, TaskTagRules>();
        }
    }
}