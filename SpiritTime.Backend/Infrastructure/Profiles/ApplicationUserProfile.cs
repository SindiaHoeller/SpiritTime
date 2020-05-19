using AutoMapper;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.Account;
using SpiritTime.Shared.Models.TagModels;

namespace SpiritTime.Backend.Infrastructure.Profiles
{
    public class ApplicationUserProfile : Profile
    {
        /// <summary>
        /// ApplicationUserProfile
        /// </summary>
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, UserInfo>();
        }
    }
}