using AutoMapper;
using SpiritTime.Shared.Models.TagModels;

namespace SpiritTime.Frontend.Profiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<TagResult, TagDto>();
            CreateMap<TagDto, TagResource>();
            CreateMap<TagResource, TagDto>();
            CreateMap<TagInfo, TagDto>();
            CreateMap<TagDto, TagInfo>();
        }
    }
}