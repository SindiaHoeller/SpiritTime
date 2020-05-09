using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.TagModels;

namespace SpiritTime.Backend.Infrastructure.Profiles
{
    /// <summary>
    /// TagProfile
    /// </summary>
    public class TagProfile : Profile
    {
        /// <summary>
        /// TagProfile
        /// </summary>
        public TagProfile()
        {
            CreateMap<Tag, TagDto>();
            CreateMap<Tag, TagInfo>();
        }
    }
}
