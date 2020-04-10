using AutoMapper;
using SpiritTime.Shared.Models.Tags;

namespace SpiritTime.Frontend.Profiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<TagResult, TagDto>();
            CreateMap<TagDto, TagResource>();
            CreateMap<TagResource, TagDto>();
        }
    }
}