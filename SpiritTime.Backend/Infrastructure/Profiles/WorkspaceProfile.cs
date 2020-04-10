using System;
using System.Collections.Generic;
using AutoMapper;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.WorkspaceModels;

namespace SpiritTime.Backend.Infrastructure.Profiles
{
    /// <summary>
    /// WorkspaceProfile
    /// </summary>
    public class WorkspaceProfile : Profile
    {
        /// <summary>
        /// WorkspaceProfile
        /// </summary>
        public WorkspaceProfile()
        {
            CreateMap<Workspace, WorkspaceResult>();
            CreateMap<Workspace, WorkspaceDto>();
            //CreateMap<List<Workspace>, List<WorkspaceDto>>();
        }
    }
}
