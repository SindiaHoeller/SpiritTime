using System;
using System.Collections.Generic;
using AutoMapper;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Backend.Infrastructure.Profiles
{
    public class WorkspaceProfile : Profile
    {
        public WorkspaceProfile()
        {
            CreateMap<Workspace, WorkspaceResult>();
            CreateMap<Workspace, WorkspaceDto>();
            //CreateMap<List<Workspace>, List<WorkspaceDto>>();
        }
    }
}
