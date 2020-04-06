using System;
using AutoMapper;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Frontend.Profiles
{
    public class WorkspaceProfile : Profile
    {
        public WorkspaceProfile()
        {
            CreateMap<WorkspaceResult, WorkspaceDto>();
            CreateMap<WorkspaceDto, WorkspaceResult>();
            CreateMap<WorkspaceDto, WorkspaceResource>();
            CreateMap<WorkspaceResource, WorkspaceDto>();
        }
    }
}
